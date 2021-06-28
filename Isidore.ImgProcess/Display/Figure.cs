using System.Windows.Forms;
using System.Drawing;

namespace Isidore.ImgProcess
{
    /// <summary>
    /// Present 2D arrays as bitmaps in individual form windows
    /// </summary>
    public class Figure : Form
    {
        # region Fields & Properties

        /// <summary>
        /// Picture box used for displaying data
        /// </summary>
        public PictureBox picBox;

        # endregion Fields & Properties

        # region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Figure()
        {
            picBox = new PictureBox();
            picBox.Dock = DockStyle.Fill;
            picBox.SizeMode = PictureBoxSizeMode.Zoom; // Keeps aspect ratio
            Controls.Add(picBox);
            Show();
        }

        /// <summary>
        /// Constructor that displays the image input
        /// </summary>
        /// <param name="img"> Data to be displayed </param>
        /// <param name="title"> Text displayed across the 
        /// banner bar </param>
        public Figure(Image img, string title)
            : this()
        {
            Disp(img, title);
        }

        # endregion Constructor

        # region Methods

        /// <summary>
        /// Displays numeric array data in the current figure
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Numeric data array </param>
        /// <param name="title"> Text displayed across the 
        /// banner bar </param>
        public void Disp<T>(T[,] arr, string title)
        {
            Bitmap img = ConvertImg.toBitmap(arr);
            Disp(img, title);
        }

        /// <summary>
        /// Displays and image instance in the current figure
        /// </summary>
        /// <param name="img"> image instance </param>
        /// <param name="title"> Text displayed across the 
        /// banner bar </param>
        public void Disp(Image img, string title)
        {
            this.ClientSize = img.Size;
            this.picBox.Size = img.Size;
            this.picBox.Image = img;
            this.Refresh();
            this.Text = title;
        }

        # endregion Methods

    }
}
