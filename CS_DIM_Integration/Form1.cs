using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CS_DIM_Integration
{
    public partial class Form1 : Form
    {
        public Form1(string str)
        {
            InitializeComponent();
            //show username
            label1.Text = str; 
            LogDisp(); 
        }
        
        public string pathString = @"C:\\Program Files\\Appointment";
        public string pathString2 = @"C:\\Program Files\\Appointment\\log.txt";
        public string pathString3 = @"C:\\Appointment2\\AppointmentData.csv";

        private void Button1_Click_submit(object sender, EventArgs e)
        {
          //  Imp_csv();           
            if (File.Exists(pathString3))
            {
                try
                {
                    File.Delete(pathString3);
                    Exp_csv();
                }
                catch (IOException ex)
                {
                    MessageBox.Show("AppointmentData.csv file is opened!" + ex.ToString());
                }
            }
            else
            {
                Exp_csv();
            }        
        }

        public void Exp_csv()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectStr.strSQLSrv))
                {
                    connection.Open();
                    //using (SqlCommand command = new SqlCommand(@"select top 10 DocCode,DocSurname,DocGiven from doctors where docSurname is not null", connection))
                    using (SqlCommand command = new SqlCommand(@"SELECT [AHistCPerCPI],[DeptDesc], 
                                                                   convert(varchar, [AHistStartTime],23) as [date], 
                                                                   convert(varchar(5),[AHistStartTime],8) as [Start Time],
                                                                   convert(varchar(5),[AHistEndTime],8) as [End Time],
                                                                   [CPerHPhone], [CPerSurname],[CPerGiven], [DocSurname], [AHistComm]
                                                                  FROM [dbo].[CS_AppointmentHistory]
                                                                     Left Join CPIPersonal on [AHistCPerCPI] = [CPerCPI] and [AHistInstn] = [CPerInstn]
                                                                     Left Join CS_DepartmentProcedures on [AHistDProcID] = [DProcID]
                                                                     Left Join Departments on [DProcDeptCode] = [DeptCode]
                                                                     Left Join Doctors on [AHistAppResDoctorInfo] = [DocCode]
                                                                  WHERE datediff(day, [AHistStartTime], '" + dateTimePicker1.Value + "') = 0 and AHistStsID != 4", connection))

                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //Check Data
                            if (reader != null && reader.HasRows)
                            {
                                Heading();
                                while (reader.Read())
                                {
                                   using (StreamWriter file = new StreamWriter(pathString3, true, Encoding.UTF8))
                                    {
                                        // file.WriteLine(reader.GetValue(0) + "," + reader.GetValue(1) + "," + reader.GetValue(2) + "," + reader.GetValue(3) + "," + reader.GetValue(4) + "," + reader.GetValue(5) + "," + reader.GetValue(6) + "," + " " + "," + reader.GetValue(7) + "," + reader.GetValue(8) + "," + reader.GetValue(9));
                                        /**  for NON-Care Patient **/
                                        string strHN = reader.GetValue(0).ToString();
                                        string strComment = reader.GetValue(9).ToString();
                                        //if CPI is Null
                                        if ((strHN == ""))
                                        {
                                            List<int> inst = new List<int>();
                                            //Check with Comma
                                            if (strComment.Contains(","))
                                            {
                                                for (int i = 0; i < strComment.Length; i++)
                                                {
                                                    if (strComment[i] == ',')
                                                    {
                                                        inst.Add(i);
                                                    }
                                                }

                                                string NameStr = strComment.Substring(0, inst[0]);
                                                string PhoneStr = strComment.Substring((inst[0] + 1), inst[1] - inst[0] - 1);
                                                string CommentStr = strComment.Substring(inst[1] + 1);

                                                //Replace with String above
                                                file.WriteLine(reader.GetValue(0) + "," + reader.GetValue(1) + "," + reader.GetValue(2) + "," + reader.GetValue(3) + "," + reader.GetValue(4) + "," + PhoneStr + "," + NameStr + "," + " " + "," + reader.GetValue(7) + "," + reader.GetValue(8) + "," + CommentStr);
                                                file.Close();
                                            }
                                            else
                                            {
                                                MessageBox.Show("This Appointment comment is not written properly! " + reader.GetValue(9).ToString());
                                                file.WriteLine(reader.GetValue(0) + "," + reader.GetValue(1) + "," + reader.GetValue(2) + "," + reader.GetValue(3) + "," + reader.GetValue(4) + "," + reader.GetValue(5) + "," + reader.GetValue(6) + "," + " " + "," + reader.GetValue(7) + "," + reader.GetValue(8) + "," + reader.GetValue(9));
                                                file.Close();
                                            }

                                        }//End of IF
                                        else
                                        {
                                            //If CPI is not Null, Do this block.
                                            file.WriteLine(reader.GetValue(0) + "," + reader.GetValue(1) + "," + reader.GetValue(2) + "," + reader.GetValue(3) + "," + reader.GetValue(4) + "," + reader.GetValue(5) + "," + reader.GetValue(6) + "," + " " + "," + reader.GetValue(7) + "," + reader.GetValue(8) + "," + reader.GetValue(9));
                                            file.Close();
                                        }

                                    }
                                }

                                //For LOG
                                if (!File.Exists(pathString2))
                                {
                                    Directory.CreateDirectory(pathString);
                                    Log();
                                }
                                else
                                {
                                    Log();
                                }
                                //All Process done here
                                MessageBox.Show("Successfully Exported!");
                                //Clear logMsg
                                richTextBox1.Clear();
                                //Display logMsg
                                LogDisp();
                                connection.Close();

                            }//EO Data Check                                               
                            else
                            {
                                MessageBox.Show("There is no Appointment Data on "+ dateTimePicker1.Text);
                                connection.Close();
                            }
                        }
                    }

                }      
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void Heading()
        {
            using (StreamWriter file = new StreamWriter(pathString3, true, Encoding.UTF8))
            {
               file.WriteLine("HN" + "," + "Location" + "," + "Appointment Date" + "," + "Start Time" + "," + "End Time" + "," + "Phone" + "," + "PatientName"+","+"PatientGiven" + "," + "Salutation" + "," + "Doctor Name" + "," + "Comment");
            }
        }
        //Not Using
        private void Imp_csv()
        {
                using (StreamWriter output = new StreamWriter(@"D:\\integration\\text.csv", true, Encoding.UTF8))
                {
                // Create and write a string containing the symbol for Pi.
                string num = "OPD";
                    string name = "702";
                    // Convert the UTF-16 encoded source string to UTF-8 and ASCII.
                    byte[] utf8Num = Encoding.UTF8.GetBytes(num);
                    byte[] utf8Name = Encoding.UTF8.GetBytes(name);
                    //  byte[] asciiString = Encoding.ASCII.GetBytes(srcString);

                    // Write the UTF-8 and ASCII encoded byte arrays. 
                    output.WriteLine(BitConverter.ToString(utf8Num) + "," + BitConverter.ToString(utf8Name));
                    // output.WriteLine("UTF-8  Bytes: {0}", BitConverter.ToString(utf8Name));
                    // output.WriteLine("ASCII  Bytes: {0}", BitConverter.ToString(asciiString));

                    label1.Text = (Encoding.UTF8.GetString(utf8Num) + (Encoding.UTF8.GetString(utf8Name)));
                    // Convert UTF-8 and ASCII encoded bytes back to UTF-16 encoded  
                    // string and write.
                    // output.WriteLine("UTF-8  Text : {0}", Encoding.UTF8.GetString(utf8String));
                    // output.WriteLine("ASCII  Text : {0}", Encoding.ASCII.GetString(asciiString));

                    // textBox2.Text = (Encoding.UTF8.GetString(utf8String));
                    // Console.WriteLine(Encoding.ASCII.GetString(asciiString));
                }
                /** For LOG
                  if (!File.Exists(pathString2))
                  {
                  Directory.CreateDirectory(pathString);
                  Log();
                  }
                  else
                  {
                  Log();          
                  }
    **/
            MessageBox.Show("Success!");
        //    richTextBox1.Clear();
         //   LogDisp();
        }//End of Imp_csv

        public void Log()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(label1.Text+"\t " + dateTimePicker1.Text +"\t " + DateTime.Now.ToString() + "\r\n");
            // flush every 20 seconds as you do it
            File.AppendAllText(pathString2, sb.ToString());
            sb.Clear();
        }

        private void LogDisp()
        {
            try
            {
                string[] lines = File.ReadAllLines(pathString2);
                
                foreach (string line in lines)
                {
                   richTextBox1.AppendText(line + "\r\n");
                }
             //   Console.WriteLine();
            }

            catch (IOException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void FormClose(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
