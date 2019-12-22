using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Grpc.Core;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using Google.Protobuf;
using gRPC_Server;
using System.IO;

namespace gRPC_Demo
{
    public partial class Form1 : Telerik.WinControls.UI.RadForm
    {
        static Channel _channel = new Channel(Properties.Settings.Default.ServerAddr, ChannelCredentials.Insecure);
        // var client = await GetClient();
       Students.StudentsClient client = new Students.StudentsClient(_channel);

        Task _lookupTask;
      //  StudentsTempProfiles _studentTemp = new StudentsTempProfiles();
        BindingSource _bs = new BindingSource();
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }
      
        async Task CheckUpAsync(string text, RadButton rbd)
        {
            radGridView1.Enabled = false;

            if (rbd.Name == "radButton3")
            {
                var studentRequested = new StudentsLookupModel { MatricNo = text };

                var student = await client.GetStudentsInfoAsync(studentRequested);
                if (student.Name.Equals(string.Empty))
                {
                    radGridView1.Enabled = true;
                    RadMessageBox.Show("Not found on Server!!!", "Alert");
                }
                else
                {
                    // student.
                    //  pictureBox1.Image = convertfrombytesarray(Convert.FromBase64String(student.ProfilePicture.ToBase64()));
                    StudentsTempProfiles studentTemp = new StudentsTempProfiles();
                    radGridView1.Enabled = true;
                    studentTemp.matricNo = student.MatricNo;
                    studentTemp.name = student.Name;
                    studentTemp.bloodGroup = student.BloodGroup;
                    studentTemp.school = student.School;
                    studentTemp.programme = student.Programme;
                    studentTemp.profilePics = convertfrombytesarray(Convert.FromBase64String(student.ProfilePicture.ToBase64()));
                    _bs.DataSource = studentTemp;
                    studentTemp = null;
                }
            }
            else if (rbd.Name == "radButton1")
            {
                List<StudentsTempProfiles> studentTempList = new List<StudentsTempProfiles>();
                using (var call = client.GetAllStudentsInfo(new StudentLookup()))
                {

                    while (await call.ResponseStream.MoveNext())
                    {
                        StudentsTempProfiles studentTemp = new StudentsTempProfiles();
                        var currentStudentObj = call.ResponseStream.Current;
                        studentTemp.matricNo = currentStudentObj.MatricNo;
                        studentTemp.name = currentStudentObj.Name;
                        studentTemp.bloodGroup = currentStudentObj.BloodGroup;
                        studentTemp.school = currentStudentObj.School;
                        studentTemp.programme = currentStudentObj.Programme;
                        studentTemp.profilePics = convertfrombytesarray(Convert.FromBase64String(currentStudentObj.ProfilePicture.ToBase64()));


                        studentTempList.Add(studentTemp);
                        //MessageBox.Show(currentStudentObj.Name);
                        _bs.DataSource = studentTempList;
                    }

                    radGridView1.Enabled = true;

                }


            }
            else if (rbd.Name == "radButton4")
            {
                var studentRequested = new StudentsLookupModel { MatricNo = text };
                var studentList = client.DeleteAStudent(studentRequested);
               
                List<StudentsTempProfiles> studentTempList = new List<StudentsTempProfiles>();
                while (await studentList.ResponseStream.MoveNext())
                {
                    StudentsTempProfiles studentTemp = new StudentsTempProfiles();
                    var currentStudentObj = studentList.ResponseStream.Current;
                    studentTemp.matricNo = currentStudentObj.MatricNo;
                    studentTemp.name = currentStudentObj.Name;
                    studentTemp.bloodGroup = currentStudentObj.BloodGroup;
                    studentTemp.school = currentStudentObj.School;
                    studentTemp.programme = currentStudentObj.Programme;
                    studentTemp.profilePics = convertfrombytesarray(Convert.FromBase64String(currentStudentObj.ProfilePicture.ToBase64()));
  

                    studentTempList.Add(studentTemp);
                    //MessageBox.Show(currentStudentObj.Name);
                    _bs.DataSource = studentTempList;
                }
                radGridView1.Enabled = true;
            
            }
           _bs.ResetBindings(false);
            radGridView1.DataSource = _bs;
            AdjustRadGridViewColumns(radGridView1);
        }
        Image convertfrombytesarray(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }

        }
        private static string ConvertImageToBase64(Image img, ImageFormat fmt)
        {
            byte[] imgarray;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, fmt);
                imgarray = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(imgarray, 0, Convert.ToInt32(ms.Length));

            }
            return Convert.ToBase64String(imgarray);


        }
        public void AdjustRadGridViewColumns(RadGridView rd)
        {
            //Ensure the base values aren't null
            if (rd == null || rd.Columns == null) return;

            //Adjust column widths and visibility
            for (int i = 0; i < rd.Columns.Count; i++)
            {
                rd.Columns[0].HeaderText = "Matric No";
                rd.Columns[1].HeaderText = "Name";
                rd.Columns[2].HeaderText = "Blood Group";
                rd.Columns[3].HeaderText = "School";
                rd.Columns[4].HeaderText = "Programme";
                rd.Columns[5].HeaderText = "Profile Picture";

            }
            radGridView1.BestFitColumns();
            if (rd.Rows.Count > 2)
            {
                rd.AllowSearchRow = true;
            }
            else
            {
                rd.AllowSearchRow = false;
            }
           
            return;
        }

        private void radMaskedEditBox1_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void radMaskedEditBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                radButton3.PerformClick();
            }
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            _lookupTask = CheckUpAsync("", sender as RadButton);

        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            if (radMaskedEditBox1.Text.Contains("_"))
            {
                RadMessageBox.Show("Invalid Matric No", "Error");
                radMaskedEditBox1.Focus();
            }
            else
            {
               _lookupTask = CheckUpAsync(radMaskedEditBox1.Text, sender as RadButton);
            }
        }

        private void radButton4_Click(object sender, EventArgs e)
        {
            if (radGridView1.SelectedRows.Count > 0)
            {
                string recordsAdminNo = radGridView1.SelectedRows[0].Cells[0].Value.ToString();
                DialogResult res = RadMessageBox.Show("Are you Sure you want to delete " + recordsAdminNo+ "'s Record", "Confirmation", MessageBoxButtons.OKCancel, RadMessageIcon.Info);
                if (res == DialogResult.OK)
                {
                    _lookupTask = CheckUpAsync(recordsAdminNo, sender as RadButton);
                    RadMessageBox.Show("Successfully deleted " + recordsAdminNo, "Notice");
                }
                else { return; }
            }
            else
            {
                RadMessageBox.Show("Pls Select a row to Delete", "Error!!!");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radButton2_Click(object sender, EventArgs e)
        {
           
        }

        private async void  radButton2_Click_1(object sender, EventArgs e)
        {
            this.Enabled = false;
            RadControl[] cont = { radTextBox1, radTextBox3, radTextBox4, radTextBox5, radTextBox6};
            if (radTextBox1.Text == string.Empty || radTextBox3.Text == string.Empty || radTextBox4.Text == string.Empty || radTextBox5.Text == string.Empty || radTextBox6.Text == string.Empty)
            {
                foreach (RadTextBox item in cont)
                {
                    if (item.Text == string.Empty)
                    {
                        item.BackColor = Color.Pink;
                    }
                }
                this.Enabled = true;
            }
            else
            {
                var studentRequested = new StudentsModel { MatricNo = radTextBox1.Text , Name = radTextBox3.Text, BloodGroup = radTextBox4.Text, School = radTextBox3.Text,Programme = radTextBox6.Text, ProfilePicture = Google.Protobuf.ByteString.FromBase64(ConvertImageToBase64(pictureBox1.Image,ImageFormat.Png)) };

                var response = await client.AddStudentAsync(studentRequested);
                if (response.Prompt == "Success")
                {
                    foreach (RadTextBox item in cont)
                    {
                        item.Text = string.Empty;
                    }
                    pictureBox1.Image = Properties.Resources.utilities_user_info;
                }
                this.Enabled = true;
                RadMessageBox.Show(response.Prompt);
            }
        }

        private void radTextBox1_TextChanged(object sender, EventArgs e)
        {
            (sender as RadTextBox).BackColor = Color.White;
        }

        private void radButton5_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.DefaultExt = "jpg";
                ofd.Title = "Browse Image Files";
                ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Image i = Image.FromFile(ofd.FileName);
                        Bitmap b;
                        resizeMethod(i, out b);
                        //  i ;
                        pictureBox1.Image = b;
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch (Exception ex)
                    { RadMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Error); }
                }
            }
        }
        public void resizeMethod(Image image__, out Bitmap x)
        {
            // Resize Image Before Uploading to DataBase
            Image image_ = image__;
            int imageHeight = image_.Height;
            int imageWidth = image_.Width;
            int maxHeight = 120;
            int maxWidth = 240;
            imageHeight = (imageHeight * maxWidth) / imageWidth;
            imageWidth = maxWidth;

            if (imageHeight > maxHeight)
            {
                imageWidth = (imageWidth * maxHeight) / imageHeight;
                imageHeight = maxHeight;
            }

            Bitmap bitmap = new Bitmap(image_, imageWidth, imageHeight);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            stream.Position = 0;
            byte[] image = new byte[stream.Length + 1];
            stream.Read(image, 0, image.Length);
            x = new Bitmap(stream);

        }
    }
}
