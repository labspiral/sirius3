using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;
#if OPENTK3
using DVec2 = OpenTK.Vector2d;
using DVec3 = OpenTK.Vector3d;
#else
using DVec2 = OpenTK.Mathematics.Vector2d;
using DVec3 = OpenTK.Mathematics.Vector3d;
#endif

namespace Demos
{
    internal enum ClipSampleKinds
    {
        Curves,
        Spiral,
        HatchedRectangle,
        NestedContours,
        Shapes,
        DataMatrixDots,
        DataMatrix,
        DataMatrixHatch,
        Text,
        TextHatch,
        TransformedGroup,
        Spatial
    }

    internal static class ClipSamples
    {
        internal static readonly string[] Names =
        {
            "2D Paths | Line, arc, ellipse, Bezier",
            "2D Paths | Spiral",
            "2D Regions | Hatched rectangle",
            "2D Regions | Nested contours / hole",
            "2D Regions | Rectangle, triangle, cross, trepan",
            "Generated | 2D Barcode / Data Matrix dots",
            "Generated | 2D Barcode / Data Matrix outline",
            "Generated | 2D Barcode / Data Matrix outline + hatch",
            "Generated | TrueType Text",
            "Generated | TrueType Text + hatch",
            "Groups | Transformed nested group",
            "3D | Polyline and points"
        };

        // Returns a detached, caller-owned source. No devices or marking jobs are created.
        // 호출자 소유의 독립 원본을 반환합니다. 장치나 마킹 작업을 생성하지 않습니다.
        internal static IEntity Create(ClipSampleKinds sample)
        {
            switch (sample)
            {
                case ClipSampleKinds.Curves:
                    return EntityFactory.CreateMixedGroup(Names[(int)sample], new IEntity[]
                    {
                        EntityFactory.CreateLine(new DVec2(-18, -7), new DVec2(18, 7)),
                        EntityFactory.CreateArc(DVec2.Zero, 14, 20, 300),
                        EntityFactory.CreateEllipse(DVec2.Zero, 30, 16),
                        new EntityBezierSpline(new[] { new DVec2(-18, 2), new DVec2(-5, 18),
                            new DVec2(5, -18), new DVec2(18, -2) })
                    });
                case ClipSampleKinds.Shapes:
                    return EntityFactory.CreateMixedGroup(Names[(int)sample], new IEntity[]
                    {
                        EntityFactory.CreateRectangle(DVec2.Zero, 30, 24),
                        new EntityTriangle(new DVec3(0, -12, 0), 32, 26),
                        new EntityCross(DVec3.Zero, 32, 24, 4),
                        new EntityTrepan(DVec3.Zero, 12, 28, 2)
                    });
                case ClipSampleKinds.NestedContours:
                    // Opposite directions make a hole for both EvenOdd and NonZero.
                    // 반대 방향은 EvenOdd와 NonZero 모두에서 구멍을 형성합니다.
                    return EntityFactory.CreateMixedGroup(Names[(int)sample], new IEntity[]
                    {
                        new EntityPolyline2D(new[] { new DVec2(-18, -14), new DVec2(18, -14),
                            new DVec2(18, 14), new DVec2(-18, 14) }, true),
                        new EntityPolyline2D(new[] { new DVec2(-5, -4), new DVec2(-5, 4),
                            new DVec2(5, 4), new DVec2(5, -4) }, true)
                    });
                case ClipSampleKinds.HatchedRectangle:
                    var rectangle = EntityFactory.CreateRectangle(DVec2.Zero, 30, 24);
                    AddSampleHatch(rectangle, 1);
                    return rectangle;
                case ClipSampleKinds.Text:
                case ClipSampleKinds.TextHatch:
                    var text = EntityFactory.CreateText("Arial", FontStyle.Bold, "BO", 24);
                    text.AlignmentXY = AlignmentXYs.MiddleCenter;
                    if (sample == ClipSampleKinds.TextHatch) AddSampleHatch(text, 0.5);
                    return text;
                case ClipSampleKinds.DataMatrixDots:
                    return EntityFactory.CreateDataMatrix("CLIP123", EntityBarcode2DBase.Barcode2DCells.Dots, 30, 24);
                case ClipSampleKinds.DataMatrix:
                case ClipSampleKinds.DataMatrixHatch:
                    var barcode = EntityFactory.CreateDataMatrix("CLIP123", EntityBarcode2DBase.Barcode2DCells.Outline, 30, 24);
                    if (sample == ClipSampleKinds.DataMatrixHatch) AddSampleHatch(barcode, 0.35);
                    return barcode;
                case ClipSampleKinds.Spiral:
                    return EntityFactory.CreateSpiral(DVec2.Zero, 36, 0, 4,
                        EntitySpiral.SpiralTypes.Archimedean, false);
                case ClipSampleKinds.TransformedGroup:
                    var child = EntityFactory.CreateMixedGroup("Rotated child", new IEntity[]
                    {
                        EntityFactory.CreateRectangle(DVec2.Zero, 30, 16),
                        EntityFactory.CreateArc(DVec2.Zero, 12, 0, 270)
                    });
                    child.ModelRotate = new DVec3(0, 0, 30);
                    var parent = EntityFactory.CreateMixedGroup(Names[(int)sample], new[] { child });
                    parent.ModelScale = new DVec3(1.2, 0.8, 1);
                    parent.ModelTranslate = new DVec3(3, 2, 0);
                    return parent;
                case ClipSampleKinds.Spatial:
                    return EntityFactory.CreateMixedGroup(Names[(int)sample], new IEntity[]
                    {
                        new EntityPolyline3D(new[] { new DVec3(-18, -8, -6), new DVec3(0, 8, 0),
                            new DVec3(18, -8, 6) }),
                        EntityFactory.CreatePoints(new[] { new DVec3(-12, 0, -4), DVec3.Zero,
                            new DVec3(8, 0, 2), new DVec3(12, 0, 4) })
                    });
                default: throw new ArgumentOutOfRangeException(nameof(sample));
            }
        }

        private static void AddSampleHatch(IHatchable entity, double interval)
        {
            var hatch = HatchFactory.CreateLine(30, interval, 0, true);
            hatch.ModelColor = new DVec3(1, 0.75, 0);
            entity.HatchMarkOption = HatchMarkOptions.HatchFirst;
            entity.AddHatch(hatch);
        }
    }
}
