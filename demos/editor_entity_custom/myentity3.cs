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
    /// Drill Holes 개체
    /// </summary>
    public class EntityDrillHoles
        : EntityTransformBaseWithChildren<IEntity>
        , IReversable
        , IRenderable
        , IHitTestable
        , IMarkerable
    {
        #region IRenderable impl
        /// <inheritdoc/>
        [Browsable(true)]
        [Category("Render")]
        [DisplayName("IsAllowRender")]
        [Description("IsAllowRender")]
        [JsonProperty]
        public virtual bool IsAllowRender { get; set; }

        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public virtual IView View { get; internal set; }

        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public virtual int BufferVersion { get; set; } = 0;
        #endregion

        #region IMarkerable impl
        /// <inheritdoc/>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Mark")]
        [DisplayName("IsAllowMark")]
        [Description("IsAllowMark")]
        [JsonProperty]
        public virtual bool IsAllowMark { get; set; }

        /// <inheritdoc/>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Mark")]
        [DisplayName("Repeats")]
        [Description("Repeats")]
        [JsonProperty]
        [Numeric(1, 100, 1)]
        public virtual int Repeats { get; set; }

        /// <summary>
        /// Not used 
        /// <para>
        /// Used each <see cref="PenColor"/> in <see cref="IHasChildren{T}.Children"/>. <br/>
        /// </para>
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual System.Drawing.Color PenColor { get; set; }

        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public virtual object MarkSyncRoot { get; private set; } = new object();
        #endregion

        #region IHitTestable impl
        /// <inheritdoc/>
        [Category("HitTest")]
        [DisplayName("IsAllowHitTest")]
        [Description("IsAllowHitTest")]
        [JsonProperty]
        public virtual bool IsAllowHitTest { get; set; }
        #endregion

        #region IReversable impl
        /// <inheritdoc/>
        [Category("Data")]
        [DisplayName("IsReversed")]
        [Description("IsReversed")]
        [JsonProperty]
        public virtual bool IsReversed { get; protected set; }
        #endregion

        [Category("Data")]
        [DisplayName("Rows")]
        [Description("Rows")]
        [JsonProperty]
        public virtual int Rows
        {
            get { return rows; }
            set {
                if (value <= 0)
                    return;
                rows = value;
                ModifyFlag.Add(ModifyFlags.Bit.Vertices);
            }
        }
        int rows;

        [Category("Data")]
        [DisplayName("Cols")]
        [Description("Cols")]
        [JsonProperty]
        public virtual int Cols
        {
            get { return cols; }
            set
            {
                if (value <= 0)
                    return;
                cols = value;
                ModifyFlag.Add(ModifyFlags.Bit.Vertices);
            }
        }
        int cols;

        [Category("Data")]
        [DisplayName("Diameter")]
        [Description("Diameter")]
        [JsonProperty]
        public virtual double Diameter
        {
            get { return diameter; }
            set
            {
                if (value <= 0)
                    return;
                diameter = value;
                ModifyFlag.Add(ModifyFlags.Bit.Vertices);
            }
        }
        double diameter;

        [Category("Data")]
        [DisplayName("Revolutions")]
        [Description("Revolutions")]
        [JsonProperty]
        public virtual int Revolutions
        {
            get { return revolutions; }
            set
            {
                if (value <= 0)
                    return;
                revolutions = value;
                ModifyFlag.Add(ModifyFlags.Bit.Vertices);
            }
        }
        int revolutions;


        [Category("Data")]
        [DisplayName("Pitch")]
        [Description("Pitch")]
        [JsonProperty]
        public virtual double Pitch
        {
            get { return pitch; }
            set
            {
                if (value < 0)
                    return;
                pitch = value;
                ModifyFlag.Add(ModifyFlags.Bit.Vertices);
            }
        }
        double pitch;


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDrillHoles"/> class with default values.
        /// <para>기본값으로 <see cref="EntityDrillHoles"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// </summary>
        public EntityDrillHoles()
            : base()
        {
            Name = $"DrillHoles_{Id}";
            IsAllowRender = true;
            IsAllowHitTest = true;
            IsAllowMark = true;            

            Repeats = 1;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDrillHoles"/> class with a specified name and a collection of child entities.
        /// <para>지정된 이름과 자식 엔티티 컬렉션으로 <see cref="EntityDrillHoles"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// </summary>
        /// <param name="rows">The number of rows of drill holes.</param>
        /// <param name="cols">The number of columns of drill holes.</param>
        /// <param name="diameter">The diameter of each drill hole.</param>
        /// <param name="pitch">The distance between the centers of adjacent drill holes.</param>
        /// <param name="revolutions">Revolutions (Default: 10)</param>
        public EntityDrillHoles(int rows, int cols, double diameter, double pitch, int revolutions = 10)
            : this()
        {
            Debug.Assert(rows > 0, "Rows must be greater than 0.");
            Debug.Assert(cols > 0, "Cols must be greater than 0.");
            Debug.Assert(diameter > 0, "Diameter must be greater than 0.");
            Debug.Assert(pitch > 0, "Pitch must be greater than 0.");

            this.Rows = rows;
            this.Cols = cols;
            this.Diameter = diameter;
            this.Pitch = pitch;
            this.Revolutions = revolutions;
        }
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name} [{ChildrenCount}]";
        }
        /// <inheritdoc/>
        public override object Clone()
        {
            var entity = new EntityDrillHoles();
            entity.CloneFrom(this);

            entity.IsAllowRender = IsAllowRender;

            entity.IsAllowMark = this.IsAllowMark;
            entity.Repeats = this.Repeats;
            entity.PenColor = this.PenColor;

            entity.IsAllowHitTest = this.IsAllowHitTest;

            entity.IsReversed = this.IsReversed;

            entity.Repeats = this.Repeats;

            entity.modelAlign = ModelAlign;
            entity.modelTranslate = ModelTranslate;
            entity.modelScale = ModelScale;
            entity.modelRotate = ModelRotate;
            entity.modelMatrix = ModelMatrix;

            entity.alignmentXy = alignmentXy;
            entity.alignmentZ = alignmentZ;
            entity.IsAllowTransform = IsAllowTransform;

            entity.OriginalMin = OriginalMin;
            entity.OriginalMax = OriginalMax;
            entity.ModelMin = ModelMin;
            entity.ModelMax = ModelMax;

            foreach (var child in this.Children)
                entity.AddChild(child.Clone() as IEntity);

            entity.rows = this.rows;
            entity.cols = this.cols;
            entity.diameter = this.diameter;
            entity.pitch = this.pitch;
            entity.revolutions = this.revolutions;
            return entity;
        }

        public bool Load()
        {
            if (Rows <= 0 || Cols <= 0 || Diameter <= 0 || Pitch < 0 || Revolutions <= 0)
                return false;

            // Clear children
            base.ClearChildren();

            // Offset for align to center
            double offsetX = ((cols - 1) * pitch) * 0.5;
            double offsetY = ((rows - 1) * pitch) * 0.5;

            List<IEntity> entities = new List<IEntity>(rows * cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Center x, y for each drill hole
                    double x = (j * pitch) - offsetX;
                    double y = (i * pitch) - offsetY;

                    var spiral = new EntitySpiralClassic(
                        new DVec3(x, y, 0),
                        diameter,
                        diameter * 0.1,
                        0,
                        revolutions,
                        true);

                    //var spiral = new EntitySpiral(
                    //    new DVec3(x, y, 0),
                    //    diameter,
                    //    diameter * 0.1, 
                    //    revolutions, 
                    //    EntitySpiral.SpiralTypes.Archimedean, 
                    //    true);

                    //var circle = new EntityArc(
                    //    new DVec3(x, y, 0),
                    //    diameter / 2.0,
                    //    0,
                    //    360 * revolutions);

                    // Add entity to list
                    entities.Add(spiral);
                }
            }

            // Reverse entities if IsReversed is true
            if (this.IsReversed)
                entities.Reverse();

            // Add entities to the children
            base.AddChildren(entities);

            // Regenerate children entities
            foreach (var e in this.Children)
                e.Regen();                

            // Edit Modify flags
            ModifyFlag.Remove(ModifyFlags.Bit.Vertices);
            //ModifyFlag.Add(ModifyFlags.Bit.AABBTree);
            ModifyFlag.Add(ModifyFlags.Bit.OriginalMinMax);
            ModifyFlag.Add(ModifyFlags.Bit.ModelMinMax);

            return true;
        }
        /// <inheritdoc/>
        public override bool Regen()
        {
            if (ModifyFlag.Contains(ModifyFlags.Bit.Vertices))
            {
                // Regen if vertices are modified
                Load();
            }

            bool success = EntityBase.InternalRegenRecursive(this);
            return success;
        }

        #region IReversable impl
        /// <inheritdoc/>
        public virtual bool Reverse()
        {
            this.IsReversed = !IsReversed;
            Children.Reverse();

            this.ClearChildren();
            this.AddChildren(Children);

            return true;
        }
        #endregion

        #region IRenderable impl
        /// <inheritdoc/>
        public virtual void Render(IView view, ICamera camera, ILight light)
        {
            View = view;
            if (ModifyFlag.IsModified())
                Regen();
            if (!this.IsAllowRender)
                return;
            if (0 == ChildrenCount)
                return;

            foreach (var e in this.Children)
            {
                if (e is IRenderable renderable)
                    renderable.Render(view, camera, light);
            }

            if (IsSelected)
                BoundBox?.Render(view, camera, light);
        }
        #endregion

        #region IHitTestable impl
        /// <inheritdoc/>
        public virtual bool HitTest(IView view, RayInfo rayInfo, out RayHitInfo? hitInfo)
        {
            hitInfo = null;
            if (!this.IsAllowHitTest)
                return false;
            if (!this.IsAllowRender)
                return false;

            if (null == BoundBox || !BoundBox.HitTest(view, rayInfo, out hitInfo))
                return false;

            foreach (var e in this.Children)
            {
                if (e is IHitTestable hitTestable)
                    if (hitTestable.HitTest(view, rayInfo, out hitInfo))
                        return true;
            }


            return false;
        }
        /// <inheritdoc/>
        public virtual bool HitTest(IView view, Frustum frustum, List<FrustumHitInfo> hitInfo)
        {
            if (!this.IsAllowHitTest)
                return false;
            if (!this.IsAllowRender)
                return false;

            if (null == BoundBox || !BoundBox.HitTest(view, frustum, null))
                return false;

            bool isHit = false;
            foreach (var e in this.Children)
            {
                if (e is IHitTestable hitTestable)
                    isHit |= hitTestable.HitTest(view, frustum, hitInfo);
            }
          
            return isHit;
        }
        #endregion

        #region IMarkerable impl
        /// <inheritdoc/>
        public virtual bool Mark(IMarker marker)
        {
            if (!this.IsAllowMark)
                return true;

            bool success = true;

            for (int i = 0; i < Repeats; i++)
            {
                foreach (var entity in this.Children)
                {
                    if (entity is IMarkerable markerable)
                    {
                        success &= markerable.Mark(marker);
                        if (!success)
                            break;
                    }
                }
                if (!success)
                    break;
            }

            return success;
        }
        #endregion
    }
}
