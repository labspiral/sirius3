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
    /// Rhombus (마름모) 개체
    /// </summary>
    public class EntityRhombus
        : EntityModelBase
        , IReversable
        , IHatchable
        , IMarkerable
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

        #region IReversable impl
        /// <inheritdoc/>
        [Category("Data")]
        [DisplayName("IsReversed")]
        [Description("IsReversed")]
        [JsonProperty]
        public virtual bool IsReversed { get; protected set; }
        #endregion

        #region IHatchable impl
        /// <inheritdoc/>
        public event EventHandler<ChildChangedEventArgs<IHatch>> HatchChanged;
        /// <summary>
        /// Raises the <see cref="HatchChanged"/> event.
        /// <para><see cref="HatchChanged"/> 이벤트를 발생시킵니다.</para>
        /// <para>引发 <see cref="HatchChanged"/> 事件。</para>
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnHatchChanged(ChildChangedEventArgs<IHatch> e)
            => HatchChanged?.Invoke(this, e);

        /// <inheritdoc/>
        [Category("Hatch")]
        [DisplayName("IsAllowHatch")]
        [Description("IsAllowHatch")]
        [JsonProperty]
        public virtual bool IsAllowHatch { get; set; }

        /// <inheritdoc/>
        [Category("Hatch")]
        [DisplayName("IsClosed")]
        [Description("IsClosed")]
        [JsonIgnore]
        public virtual bool IsClosed
        {
            get { return Vertices.Count > 0; }
        }

        /// <summary>
        /// Gets or sets the list of <see cref="IHatch"/> objects associated with this entity.
        /// <para>이 엔티티와 연결된 <see cref="IHatch"/> 객체 목록을 가져오거나 설정합니다.</para>
        /// <para>获取或设置与此实体关联的 <see cref="IHatch"/> 对象列表。</para>
        /// </summary>
        [Browsable(false)]
        [JsonProperty]
        protected virtual List<IHatch> HatchList { get; set; } = new List<IHatch>();

        /// <inheritdoc/>
        [RefreshProperties(RefreshProperties.All)]
        [Category("Hatch")]
        [DisplayName("HatchList")]
        [Description("HatchList")]
        [Editor(typeof(HatchEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HatchListConverter))]
        [JsonIgnore]
        public virtual IReadOnlyList<IHatch> Hatches
        {
            get { return HatchList; }
            set
            {
                if (value == null)
                {
                    ClearHatches(true);
                }
                else
                {
                    ClearHatches(true);
                    AddHatches(value);
                }
                Regen();
            }
        }

        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public virtual int HatchCount { get { return HatchList.Count; } }

        /// <inheritdoc/>
        [Category("Hatch")]
        [DisplayName("HatchMarkOption")]
        [Description("HatchMarkOption")]
        [JsonProperty]
        public virtual HatchMarkOptions HatchMarkOption { get; set; }
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

        /// <inheritdoc/>
        [Category("Mark")]
        [DisplayName("PenColor")]
        [Description("PenColor")]
        [Editor(typeof(PenColorEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public virtual System.Drawing.Color PenColor
        {
            get
            {
                var modelColor = System.Drawing.Color.FromArgb(
                    255,
                    (int)(ModelColor.X * 255.0),
                    (int)(ModelColor.Y * 255.0),
                    (int)(ModelColor.Z * 255.0));

                foreach (var c in SpiralLab.Sirius3.UI.Config.EntityPenColors)
                {
                    if (c.ToArgb() == modelColor.ToArgb())
                    {
                        return c; 
                    }
                }
                return modelColor;
            }
            set
            {
                Alpha = value.A / 255.0;
                ModelColor = new DVec3(
                    value.R / 255.0,
                    value.G / 255.0,
                    value.B / 255.0);
            }
        }

        /// <inheritdoc/>
        [Browsable(false)]
        [JsonIgnore]
        public virtual object MarkSyncRoot { get; private set; } = new object();
        #endregion

        [Browsable(false)]
        [JsonProperty]
        public List<DVec2> Points { get; private set; } = new List<DVec2>(4);


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRhombus"/> class with default values.
        /// <para>기본값으로 <see cref="EntityRhombus"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// </summary>
        public EntityRhombus()
           : base()
        {
            Name = $"Rhombus_{Id}";
            IsAllowRender = true;
            IsAllowHitTest = true;
            IsAllowHatch = true;
            IsAllowMark = true;
            Repeats = 1;
            HatchMarkOption = HatchMarkOptions.HatchLast;

            RenderMode = RenderModes.LineStrip;
            //override
            AABBTree = new AABBTreeLineStrip(this);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRhombus"/> class with specified parameters.
        /// <para>지정된 매개변수로 <see cref="EntityRhombus"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// </summary>
        /// <param name="p1">P1</param>
        /// <param name="p2">P1</param>
        /// <param name="p3">P1</param>
        /// <param name="p4">P1</param>
        public EntityRhombus(DVec2 p1, DVec2 p2, DVec2 p3, DVec2 p4)
             : this()
        {
            this.Points.Add(p1);
            this.Points.Add(p2);
            this.Points.Add(p3);
            this.Points.Add(p4);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name}";
        }
        /// <inheritdoc/>
        public override object Clone()
        {
            var entity = new EntityRhombus();
            entity.CloneFrom(this);

            entity.IsReversed = this.IsReversed;

            entity.IsAllowHatch = this.IsAllowHatch;
            entity.HatchMarkOption = this.HatchMarkOption;

            entity.IsAllowMark = this.IsAllowMark;
            entity.Repeats = this.Repeats;
            entity.PenColor = this.PenColor;

            //entity.Load();
            return entity;
        }
        /// <summary>
        /// Loads or reloads the rhombus geometry based on its properties.
        /// </summary>
        /// <returns><c>true</c> if the geometry was successfully loaded; otherwise, <c>false</c>.</returns>
        public bool Load()
        {
            Debug.Assert(Points.Count == 4);
            
            ClearBuffers();

            foreach (var p in Points)
                GeneratePoint(Vertices, Colors, Normals, Indices, new DVec3(p), ModelColor);
            GeneratePoint(Vertices, Colors, Normals, Indices, new DVec3(Points[0]), ModelColor); // add first to close loop

            ModifiedBuffers();

            ModifyFlag.Remove(ModifyFlags.Bit.Vertices);
            ModifyFlag.Add(ModifyFlags.Bit.AABBTree);
            ModifyFlag.Add(ModifyFlags.Bit.OriginalMinMax);
            ModifyFlag.Add(ModifyFlags.Bit.ModelMinMax);

            if (!this.IsClosed)
                ClearHatches(true);

            // 해치데이타 Regen 되도록 전파
            foreach (var hatch in HatchList)
                hatch.IsModified = true;
            return true;
        }
        /// <inheritdoc/>
        public override bool Regen()
        {
            if (ModifyFlag.Contains(ModifyFlags.Bit.Vertices))
                Load();

            return base.Regen();
        }

        #region IReversable impl
        /// <inheritdoc/>
        public virtual bool Reverse()
        {
            this.IsReversed = !IsReversed;

            ModifyFlag.Add(ModifyFlags.Bit.Vertices);
            ModifyFlag.Add(ModifyFlags.Bit.AABBTree);
            return true;
        }
        #endregion

        #region IHatchable impl
        /// <inheritdoc/>
        public virtual bool GetOriginalContours(List<Contour> contours)
        {
            if (Vertices.Count <= 2)
                return false;

            // x,y 성분만 사용 (2D only)
            var verts = Vertices.Select(v => new DVec2(v.X, v.Y)).ToList();
            var contour = new Contour(verts);
            contour.CalculateClosed();
            contours.Add(contour);
            return true;
        }

        /// <inheritdoc/>
        public virtual void AddHatch(IHatch hatch) => AddHatches(new[] { hatch });
        /// <inheritdoc/>
        public virtual void InsertHatch(int index, IHatch hatch) => InsertHatches(index, new[] { hatch });
        /// <inheritdoc/>
        public virtual bool RemoveHatch(IHatch hatch)
        {
            int idx = HatchList.IndexOf(hatch);
            if (idx < 0) return false;
            RemoveHatches(idx, 1);
            return true;
        }
        /// <inheritdoc/>
        public virtual bool RemoveHatch(int index, out IHatch hatch)
        {
            hatch = default!;
            if (index < 0 || index >= HatchList.Count) return false;
            hatch = HatchList[index];
            RemoveHatches(index, 1);
            return true;
        }
        /// <inheritdoc/>
        public virtual void AddHatches(IEnumerable<IHatch> hatches)
        {
            var list = hatches.ToList();
            if (list.Count == 0) return;

            int index = HatchList.Count;
            HatchList.AddRange(list);

            foreach (var item in HatchList)
                if (item is IEntity entity)
                    entity.Owner = this;

            ModifyFlag.Add(ModifyFlags.Bit.All);
            OnHatchChanged(new ChildChangedEventArgs<IHatch>(ChildChangeKind.Add, index, list));
        }
        /// <inheritdoc/>
        public virtual void InsertHatches(int index, IEnumerable<IHatch> items)
        {
            var list = items.ToList();
            if (list.Count == 0) return;

            HatchList.InsertRange(index, list);

            foreach (var item in HatchList)
                if (item is IEntity entity)
                    entity.Owner = this;

            ModifyFlag.Add(ModifyFlags.Bit.All);
            OnHatchChanged(new ChildChangedEventArgs<IHatch>(ChildChangeKind.Insert, index, list));
        }
        /// <inheritdoc/>
        public virtual void RemoveHatches(int index, int count)
        {
            if (count <= 0) return;
            count = Math.Min(count, HatchList.Count - index);
            if (count <= 0) return;

            HatchList.RemoveRange(index, count);
            ModifyFlag.Add(ModifyFlags.Bit.All);
            OnHatchChanged(new ChildChangedEventArgs<IHatch>(ChildChangeKind.Remove, index, items: Array.Empty<IHatch>(), count: count));
        }
        /// <inheritdoc/>
        public virtual void ReplaceHatches(int index, IEnumerable<IHatch> newItems)
        {
            var list = newItems.ToList();

            // 대량 교체는 Reset이 가장 단순/빠름
            if (list.Count > HatchList.Count / 2)
            {
                HatchList.Clear();
                HatchList.AddRange(list);
                ModifyFlag.Add(ModifyFlags.Bit.All);
                OnHatchChanged(ChildChangedEventArgs<IHatch>.Reset());
                return;
            }

            // 부분 교체: 가능한 만큼 덮어쓰고 나머지는 삽입/삭제
            int replaceCount = Math.Min(list.Count, Math.Max(0, HatchList.Count - index));
            for (int i = 0; i < replaceCount; i++)
                HatchList[index + i] = list[i];

            if (list.Count > replaceCount)
                HatchList.InsertRange(index + replaceCount, list.Skip(replaceCount));
            else if (list.Count < replaceCount)
                HatchList.RemoveRange(index + list.Count, replaceCount - list.Count);

            foreach (var item in HatchList)
                if (item is IEntity entity)
                    entity.Owner = this;

            ModifyFlag.Add(ModifyFlags.Bit.All);
            OnHatchChanged(new ChildChangedEventArgs<IHatch>(ChildChangeKind.Replace, index, list));
        }
        /// <inheritdoc/>
        public virtual void MoveHatches(int oldIndex, int count, int newIndex)
        {
            if (count <= 0) return;
            count = Math.Min(count, HatchList.Count - oldIndex);
            if (count <= 0) return;

            // 이벤트용: 원본 좌표(보정 전)로 고정
            int destOriginal = newIndex;

            // 실제 조작용: 제거 후 당김 보정
            var slice = HatchList.GetRange(oldIndex, count);
            HatchList.RemoveRange(oldIndex, count);

            int destAdjusted = destOriginal;
            if (destAdjusted > oldIndex) destAdjusted -= count; // 당김 보정

            HatchList.InsertRange(destAdjusted, slice);

            OnHatchChanged(new ChildChangedEventArgs<IHatch>(
                ChildChangeKind.Move,
                index: destOriginal,                       
                items: Array.Empty<IHatch>(),
                oldIndex: oldIndex,
                count: count));
        }
        /// <inheritdoc/>
        public virtual bool ClearHatches(bool dispose)
        {
            if (HatchList.Count == 0) return false;
            foreach (var hatch in HatchList)
                hatch?.Dispose();
            HatchList.Clear();
            OnHatchChanged(ChildChangedEventArgs<IHatch>.Reset());
            return true;
        }
        #endregion

        #region IRenderable impl

        #endregion

        #region IHitTestable impl

        #endregion

        #region IMarkerable impl
        /// <inheritdoc/>
        public virtual bool Mark(IMarker marker)
        {
            Debug.Assert(null != marker);
            if (!IsAllowMark)
                return true;
            Debug.Assert(ModifyFlag.IsEmpty());

            if (Vertices.Count == 0)
                return true;

            bool success = true;
            var rtc = marker.Scanner;
            var laser = marker.Laser;
            var markerBase = marker as MarkerBase;
            Debug.Assert(markerBase != null);

            var scanner = rtc as IScanner;
            var scanner3D = rtc as IScanner3D;

            switch (HatchMarkOption)
            {
                case HatchMarkOptions.HatchFirst:
                    success &= MarkHatch(marker);
                    if (success)
                    {
                        success &= MarkerBase.MarkEntityPen(marker, this, out var entityPen);
                        success &= MarkInternal(marker);
                    }
                    break;
                case HatchMarkOptions.HatchLast:
                    {
                        success &= MarkerBase.MarkEntityPen(marker, this, out var entityPen);
                        success &= MarkInternal(marker);
                        if (success)
                            success &= MarkHatch(marker);
                    }
                    break;
                case HatchMarkOptions.HatchOnly:
                    success &= MarkHatch(marker);
                    break;
            }
            return success;
        }
        private bool MarkInternal(IMarker marker)
        {
            bool success = true;
            var rtc = marker.Scanner as IRtc;
            var laser = marker.Laser;
            var markerBase = marker as MarkerBase;
            Debug.Assert(markerBase != null);

            var scanner = rtc as IScanner;
            var scanner3D = rtc as IScanner3D;

            var matrix = CalculateMatriesRecursive();
            try
            {
                rtc.MatrixStack.Push(matrix);
                for (int i = 0; i < Repeats; i++)
                {
                    success &= scanner3D.ListJumpTo(Vertices.First());
                    for (int j = 1; j < Vertices.Count; j++)
                    {
                        success &= scanner3D.ListMarkTo(Vertices[j]);
                        if (!success)
                            break;
                    }
                    if (!success)
                        break;
                }
            }
            finally
            {
                rtc.MatrixStack.Pop();
            }
            return success;
        }
        private bool MarkHatch(IMarker marker)
        {
            bool success = true;
            if (this is IHatchable hatchable)
            {
                if (hatchable.IsAllowHatch)
                {
                    foreach (var hatch in hatchable.Hatches)
                    {
                        if (null != hatch.HatchResult)
                        {
                            if (hatch.HatchResult is IRenderable hatchRenderable)
                            {
                                var hatchEntity = hatchRenderable as IEntity;
                                hatchEntity.Owner = this;
                                if (hatchEntity is IMarkerable hatchMarkerable)
                                {
                                    for (int i = 0; i < hatch.HatchRepeats; i++)
                                    {
                                        success &= hatchMarkerable.Mark(marker);
                                        if (!success)
                                            break;
                                    }
                                }
                            }
                        }
                        if (!success)
                            break;
                    }
                }
            }
            return success;
        }

        #endregion
    }
}
