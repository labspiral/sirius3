using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpiralLab.Sirius3.Entity;
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

namespace Demos
{
    internal class MyCamera : IStitchedImageSource
    {
        /// <inheritdoc/>
        public event EventHandler<StitchedImageReceivedEventArgs> ImageReceived;

        /// <inheritdoc/>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("Index")]
        [Description("Index of the Camera.")]
        public int Index { get; set; }

        /// <inheritdoc/>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("Name")]
        [Description("Name of the Camera.")]
        public string Name { get; set; }

        /// <inheritdoc/>
        [Browsable(false)]
        public object Tag { get; set; }

        public const int WidthPixels = 3000;
        public const int HeightPixels = 3000;
        public const double FovWidth = 20.0;
        public const double FovHeight = 20.0;

        public const int Rows = 2;
        public const int Cols = 2;


        /// <summary>
        /// Constructor of MyCamera class.
        /// </summary>
        /// <param name="index">Index of the camera.</param>
        /// <param name="name">Name of the camera.</param>
        public MyCamera(int index, string name)
        {
            Index = index;
            Name = name;
        }

        void NotifyGrabbedImage(int row, int column, Bitmap bitmap)
        {
            // some pre-processing of the grabbed image can be done here.
            // 취득된 이미지에 대한 전처리를 여기서 수행할 수 있습니다.


            ImageReceived?.Invoke(this, new StitchedImageReceivedEventArgs(row, column, bitmap));
        }

        /// <summary>
        /// Grab image from a camera 
        /// <para>
        /// 카메라에서 이미지를 가져오기
        /// </para>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool Grab(int row, int col)
        {
            // The asynchronously acquired camera image should be delivered through the ImageReceived event.
            // 비동기적으로 취득된 카메라 이미지를 ImageReceived 이벤트를 통해 전달해주어야 합니다.
            //


            return true;
        }

        /// <summary>
        /// Simulate continous grabbing an image from a camera 
        /// </summary>
        /// <returns>True if images were successfully grabbed; otherwise, false.</returns>
        public bool MoveAndGrabs(EntityStitchedImage entityStitchedImage, IScanner scanner)
        {
            // 스캐너를 이용하여 스티치 이미지의 각 타일 중심으로 이동 후 카메라에서 이미지를 취득하는 시뮬레이션
            // Simulate moving the scanner to the center of each tile of the stitched image and grabbing images from a camera.
            //var rtc = scanner as IRtc;
            //for (int row = 0; row < Rows; row++)
            //{
            //    for (int col = 0; col < Cols; col++)
            //    {
            //        DVec2 fovCenter = entityStitchedImage.GetImageCenter(row, col);
            //        rtc.CtlMoveTo(fovCenter, 1_000); // Move scanner to the center of the stitched image tile (1000 mm/s)
            //        Thread.Sleep(1); // Delay for safe movement. (1 ms)
            //        this.Grab(row, col); // Grab image from a camera
            //    }
            //}



            // In this demo, instead of grabbing images from a camera, we simulate grabbing images from a camera by opening image files from the file system.
            // 이 데모에서는 카메라로부터 이미지를 취득하는 대신, 파일 시스템에서 이미지 파일을 열어 카메라에서 이미지를 가져오는 것을 시뮬레이션합니다.
            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Title = $"Open {Rows*Cols}Image Files with {WidthPixels}x{HeightPixels} Size";
            dlg.Multiselect = true;
            dlg.Filter = "Image files (jpg, jpeg, bmp, png, gif, tif, tiff)|*.jpg;*.jpeg;*.bmp;*.png;*.gif;*.tif;*.tiff|All files (*.*)|*.*";
            if (dlg.ShowDialog() != DialogResult.OK)
                return false;

            Debug.Assert(dlg.FileNames.Length == Rows * Cols, $"You must select {Rows * Cols} images.");
            try
            {
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {
                    int row = i / Cols;
                    int col = i % Cols;

                    using Image image = Image.FromFile(dlg.FileNames[i]);
                    Debug.Assert(image.Width == WidthPixels && image.Height == HeightPixels, $"Image size must be {WidthPixels}x{HeightPixels} pixels.");
                    using Bitmap bitmap = new Bitmap(image);
                    NotifyGrabbedImage(row, col, bitmap);
                }
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }


    }
}
