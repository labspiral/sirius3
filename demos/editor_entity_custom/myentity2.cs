using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SpiralLab.Sirius3.Converter;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.View;
using SpiralLab.Sirius3.View.Camera;
using SpiralLab.Sirius3.View.Light;
using SpiralLab.Sirius3.Entity.AABB;
using System.Diagnostics;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;

#if OPENTK3
using OpenTK;
using DVec2 = OpenTK.Vector2d;
using DVec3 = OpenTK.Vector3d;
using DVec4 = OpenTK.Vector4d;
using DMat3 = OpenTK.Matrix3d;
using DMat4 = OpenTK.Matrix4d;
#elif OPENTK4
using OpenTK.Mathematics;
using DVec2 = OpenTK.Mathematics.Vector2d;
using DVec3 = OpenTK.Mathematics.Vector3d;
using DVec4 = OpenTK.Mathematics.Vector4d;
using DMat3 = OpenTK.Mathematics.Matrix3d;
using DMat4 = OpenTK.Mathematics.Matrix4d;
#endif
using OpenTK.Graphics.OpenGL;

namespace Demos
{
    /// <summary>
    /// Fiducial (피두셜) 개체
    /// </summary>
    public class EntityFiducial
        : EntityModelBase
        //, IMarkerable
    {
        #region IRenderData impl with vertices, normals, colors, ... and indices 
        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public override List<DVec3> Vertices { get; protected set; } = new List<DVec3>();
        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public override List<DVec3> Colors { get; protected set; } = new List<DVec3>();
        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public override List<DVec3> Normals { get; protected set; } = new List<DVec3>();
        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public override List<DVec2> Textures { get; protected set; } = new List<DVec2>();
        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public override List<uint> Indices { get; protected set; } = new List<uint>();
        #endregion

        #region ISliceable impl (not supported and so hide)
        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public override bool IsPreviewSlice { get; set; }

        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public override double SliceZ { get; set; } = 0;
        #endregion

       
        /// <summary>
        /// Gets or sets the center coordinates (X, Y, Z) of the fiducial.
        /// <para>중심 좌표(X, Y, Z)를 가져오거나 설정합니다.</para>
        /// </summary>
        [Category("Data")]
        [DisplayName("Center")]
        [Description("Center")]
        [TypeConverter(typeof(OpenTKVector3dConverter))]
        [JsonProperty]
        public virtual DVec3 Center
        {
            get { return center; }
            set
            {
                center = value;
                ModifyFlag.Add(ModifyFlags.Bit.Vertices);
                ModifyFlag.Add(ModifyFlags.Bit.AABBTree);
                ModifyFlag.Add(ModifyFlags.Bit.OriginalMinMax);
                ModifyFlag.Add(ModifyFlags.Bit.ModelMinMax);
            }
        }
        private DVec3 center;

        /// <summary>
        /// Gets or sets the size (in mm) of the fiducial.
        /// <para>피두셜의 크기(mm)을 가져오거나 설정합니다.</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("Size")]
        [Description("Size")]
        [TypeConverter(typeof(DoubleTypeConverter))]
        public double Size
        {
            get { return size; }
            set
            {
                if (value <= 0)
                    return;
                size = value;

                ModifyFlag.Add(ModifyFlags.Bit.Vertices);
                ModifyFlag.Add(ModifyFlags.Bit.AABBTree);
                ModifyFlag.Add(ModifyFlags.Bit.OriginalMinMax);
                ModifyFlag.Add(ModifyFlags.Bit.ModelMinMax);
            }
        }
        private double size;

        EntityArc arc;
        EntityRectangle rectangle;
        private bool disposed = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFiducial"/> class with default values.
        /// <para>기본값으로 <see cref="EntityFiducial"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// </summary>
        public EntityFiducial()
           : base()
        {
            Name = $"Fiducial_{Id}";
            IsAllowRender = true;
            IsAllowHitTest = true;

            //IsAllowHatch = true;
            //IsAllowMark = true;
            //Repeats = 1;

            // The three internal arcs own the rendered geometry and hit-test
            // acceleration structures; the trepan parent owns logical bounds.
            DisableOwnAABBTree();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFiducial"/> class with specified parameters.
        /// <para>지정된 매개변수로 <see cref="EntityFiducial"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// </summary>
        /// <param name="center">The center coordinates of the fiducial.</param>
        /// <param name="size">The size of the fiducial.</param>
        public EntityFiducial(DVec3 center, double size)
            : this()
        {
            Debug.Assert(size > 0);
            Center = center;
            Size = size;
        }
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name}";
        }
        /// <inheritdoc/>
        public override object Clone()
        {
            var entity = new EntityFiducial();
            entity.CloneFrom(this);

            entity.center = this.center;
            entity.size = this.size;

            return entity;
        }
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    arc?.Dispose();
                    rectangle?.Dispose();
                }
                disposed = true;
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Loads or reloads the fiducial geometry based on its properties.
        /// <para>피두셜의 속성을 기반으로 트레판 형상을 로드하거나 다시 로드합니다.</para>
        /// </summary>
        /// <returns><c>true</c> if the geometry was successfully loaded; otherwise, <c>false</c>.</returns>
        public bool Load()
        {
            ClearBuffers();
            rectangle?.Dispose();
            arc?.Dispose();

            rectangle = new EntityRectangle(this.center, this.size, this.size);
            rectangle.Owner = this;
            rectangle.ModelColor = this.ModelColor;
            rectangle.Regen();

            arc = new EntityArc(this.center, this.size * 0.25);
            arc.Owner = this;
            arc.ModelColor = this.ModelColor;
            arc.Regen();

            OriginalMin = new DVec3(center.X - size/2.0 , center.Y - size / 2.0, center.Z);
            OriginalMax = new DVec3(center.X + size / 2.0, center.Y + size / 2.0, center.Z);

            ModifiedBuffers();

            ModifyFlag.Remove(ModifyFlags.Bit.Vertices);
            ModifyFlag.Add(ModifyFlags.Bit.AABBTree);
            ModifyFlag.Remove(ModifyFlags.Bit.OriginalMinMax);
            ModifyFlag.Add(ModifyFlags.Bit.ModelMinMax);

            return true;
        }
        /// <inheritdoc/>
        public override bool CalculateOriginalMinMax(out DVec3 min, out DVec3 max)
        {
            min = max = DVec3.Zero;
            if (!OriginalMin.HasValue || !OriginalMax.HasValue)
                return false;
            min = OriginalMin.Value;
            max = OriginalMax.Value;
            return true;
        }
        /// <inheritdoc/>
        public override bool Regen()
        {
            if (ModifyFlag.Contains(ModifyFlags.Bit.Vertices))
                Load();

            return base.Regen();
        }

        #region IRenderable impl
        /// <inheritdoc/>
        public override void Render(IView view, ICamera camera, ILight light)
        {
            View = view;
            if (ModifyFlag.IsModified())
                Regen();

            if (!this.IsAllowRender)
                return;

            if (null == arc || null == rectangle)
                return;

            rectangle?.Render(view, camera, light);
            arc?.Render(view, camera, light);

        }
        #endregion

        #region IHitTestable impl
        /// <inheritdoc/>
        public override bool HitTest(IView view, RayInfo rayInfo, out RayHitInfo? hitInfo)
        {
            hitInfo = null;
            if (!this.IsAllowHitTest)
                return false;
            if (!this.IsAllowRender)
                return false;
            if (null == arc || null == rectangle)
                return false;

            if (null == BoundBox || !BoundBox.HitTest(view, rayInfo, out hitInfo))
                return false;
          
            if (rectangle.HitTest(view, rayInfo, out var hitInfo2))
            {
                hitInfo = hitInfo2;
                return true;
            }

            if (arc.HitTest(view, rayInfo, out var hitInfo3))
            {
                hitInfo = hitInfo3;
                return true;
            }

            return false;
        }
        /// <inheritdoc/>
        public override bool HitTest(IView view, Frustum frustum, List<FrustumHitInfo> hitInfo)
        {
            if (!this.IsAllowHitTest)
                return false;
            if (!this.IsAllowRender)
                return false;
            if (null == arc || null == rectangle)
                return false;
            if (null == BoundBox || !BoundBox.HitTest(view, frustum, null))
                return false;

            bool isHit = false;
            isHit |= rectangle.HitTest(view, frustum, hitInfo);
            if (isHit) 
                return true;
            isHit |= arc.HitTest(view, frustum, hitInfo);

            return isHit;
        }
        #endregion
      
    }
}
