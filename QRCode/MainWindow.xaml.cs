using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QRCoder;
using QRCode;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace QRCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private BitmapImage BitmapToImageSource(Bitmap bitmap)
        //{
        //    using (MemoryStream memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        //        memory.Position = 0;
        //        BitmapImage bitmapimage = new BitmapImage();
        //        bitmapimage.BeginInit();
        //        bitmapimage.StreamSource = memory;
        //        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapimage.EndInit();

        //        return bitmapimage;
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                validateAllFields();
                var image = generateQRcode();

                // Change form
                DrawImageOnscreen(image, imgQRCode);
                btnGenerate.Visibility = Visibility.Hidden;
                lbMessage.Content = "QR code generated";
                btnRefresh.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void validateAllFields()
        {
            try
            {
                validatePhone(textPhoneNumber.Text);
                validateEmail(textEmail.Text);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void validatePhone(string input)
        {
            string validPhoneRegex = @"^((\+\d{1,3}(-| )?\(?\d\)?(-| )?\d{1,5})|(\(?\d{2,6}\)?))(-| )?(\d{3,4})(-| )?(\d{4})(( x| ext)\d{1,5}){0,1}$";
            if (!validateRegex(input, validPhoneRegex))
            {
                throw new Exception("Not a valid phone number");
            }

        }

        private Boolean validateRegex(string textToValidate, string regexToMatch)
        {
            if (Regex.IsMatch(textToValidate, regexToMatch))
            {
                return true;
            }
            return false;
        }

        
        private void validateEmail(string input)
        {
            string validEmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            if (!validateRegex(input, validEmailRegex))
            {
                throw new Exception("Not a valid email");
            }
        }

        private System.Drawing.Image generateQRcode()
        {
            var textToEncode = generateVCardTextToEncode();
            var image = createQRCode(textToEncode);

            var filePath = System.Environment.CurrentDirectory +"\\"+ generateFileName();
            image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            //Process.Start(filePath);
            return image;

        }
        private string generateVCardTextToEncode()
        {

            string vCardText = "BEGIN:VCARD\r\nVERSION:2.1\r\nN:";

            vCardText += textFirstname.Text +" "+ textLastname.Text + "\r\n";

            vCardText += "EMAIL:" + textEmail.Text + "\r\n";

            vCardText += "TEL:" + textPhoneNumber.Text + "\r\n";

            vCardText += "END:VCARD";

            return vCardText;
        }
        private string generatePlainTextToEncode()
        {
            StringBuilder textToEncode = new StringBuilder();
            textToEncode.Append(textFirstname.Text).
                Append("\n").Append(textLastname.Text).
                Append("\n").Append(textPhoneNumber.Text).
                Append("\n").Append(textEmail.Text);
            return textToEncode.ToString();
        }

        // Using Zen.Barcode
        private System.Drawing.Image createQRCode2(string textToEncode)
        {
            Zen.Barcode.CodeQrBarcodeDraw brcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;
            
            return brcode.Draw(textToEncode, 50);
        }

        // Using QRCoder
        private System.Drawing.Bitmap createQRCode(string textToEncode)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
            return qrCode.GetGraphic(20);

        }

        private string generateFileName()
        {
            return textFirstname.Text.ToLower() + "-" + textLastname.Text.ToLower()+".png";
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            btnRefresh.Visibility = Visibility.Hidden;
            textFirstname.Text = "";
            textLastname.Text = "";
            textPhoneNumber.Text = "";
            lbMessage.Content = "";
            textEmail.Text = "";
            btnGenerate.Visibility = Visibility.Visible;
        }

        private void DrawImageOnscreen(System.Drawing.Image picture, System.Windows.Controls.Image dest)
        {
            // ImageSource ...

            BitmapImage bi = new BitmapImage();

            bi.BeginInit();

            MemoryStream ms = new MemoryStream();

            // Save to a memory stream...

            picture.Save(ms, ImageFormat.Bmp);

            // Rewind the stream...

            ms.Seek(0, SeekOrigin.Begin);

            // Tell the WPF image to use this stream...

            bi.StreamSource = ms;

            bi.EndInit();
            dest.Source = bi;
        }



    }
}
