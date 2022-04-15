using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;

namespace MouseUI
{
    public partial class Form3 : Form
    {
        /*
        未來的格式：
        SPO2:xx.xxx
        HR:xx.xxxx
        TEMP:xx.xxxx
        BME:xx.xx,xx.xx,xx.xx
        BME_EN:xx.xx,xx.xx,xx.xx*/
        public static double temp;
        public Form3()
        {
            InitializeComponent();
            findPorts();
            
        }

       



        private void Initport()
        {
            if (comboBox1.Text == "" || comboBox2.Text == "") //未選擇時
            {
                MessageBox.Show("please chose Port and Rate");
            }
            else
            {

                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = int.Parse(comboBox2.Text);
                serialPort1.Open();
                progressBar1.Value = 100;
            }
        }

        private void findPorts()
        {
            string[ ] portArray = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(portArray);

        }


        private void button_write_Click(object sender, EventArgs e)
        {
            var records = new List<PPG>
            {
                new PPG{ ppg1 = 1, ppg2 = 2, ppg3 = 3},
                new PPG{ ppg1 = 4, ppg2 = 5, ppg3 = 6},
            };

            using (StreamWriter writer = new StreamWriter("D:\\ProjectUI\\rawdata.csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                    MessageBox.Show("儲存完畢");
                }
            }
        }


        //ppg訊號三個值的class
        public class PPG 
        { 
            public int ppg1 { get; set; }
            public int ppg2 { get; set; }
            public int ppg3 { get; set; }
        }
        public class person
        {
            //public int id { get; set; }
            //public string name { get; set; }
            public string age { get; set; }
            public string height { get; set; }
            public string weight { get; set; }
            public string spo2 { get; set; }
            public string hand_temp { get; set; }
            public string hand_humidity { get; set; }
            public string hand_pressure { get; set; }
            public string envir_temp { get; set; }
            public string envir_humidity { get; set; }
            public string envir_pressure { get; set; }
            public string hr { get; set; }
            public string ear_temp { get; set; }

            //public double HR { get; set; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Initport();
            
        //要讀取的個數是num
            int num = 1000;
            progressBar2.Value = 0;
            if (this.serialPort1.IsOpen && tx_filepath.Text != "")
            {
                //MessageBox.Show("正在讀取");
                //建立空的寫入csv的list
                var records = new List<PPG>
                {

                };
                //讀取num個串口的信號
                for (int i = 0; i < num; i++)
                {
                    try
                    {

                        String ppgString = serialPort1.ReadLine();
                        String[] ppgArray2 = ppgString.Split(':');
                        switch (ppgArray2[0]) {
                            case "ppg":
                                string[] ppgArray3 = ppgArray2[1].Split(',');
                                int ppg1Value = int.Parse(ppgArray3[0]);
                                int ppg2Value = int.Parse(ppgArray3[1]);
                                int ppg3Value = int.Parse(ppgArray3[2]);
                                records.Add(new PPG { ppg1 = ppg1Value, ppg2 = ppg2Value, ppg3 = ppg3Value });
                                progressBar2.Value = i / 10;
                                break;
                        }

                    }
                    catch
                    {

                    }
                }

               

                using (StreamWriter writer = new StreamWriter(this.tx_filepath.Text))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(records);
                        progressBar2.Value = 100;
                        MessageBox.Show("儲存完畢");
                        serialPort1.Close();
                       

                    }
                }
                BindCsv(this.tx_filepath.Text);
                //progressBar2.Value = 100;

            }
            else 
            {
                if (this.tx_filepath.Text == "")
                    MessageBox.Show("請選擇文件");
                else
                    MessageBox.Show("請填寫基本信息");
                serialPort1.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.tx_age.Text != ""  && this.tx_filepath.Text != "")
            {
                Initport();
                if (this.serialPort1.IsOpen)
                {
                    person ps = new person();
                    var records = new List<person>
                    {

                    };
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false,
                    };
                    Boolean b_SPO2 = false;
                    Boolean b_BME = false;
                    Boolean b_BME_EM = false;
                    Boolean HR = false;
                    ps.age = this.tx_age.Text;
                    ps.height = this.tx_height.Text;
                    ps.weight = this.tx_weight.Text;
                    while (b_SPO2 == false || b_BME == false || b_BME_EM == false || HR == false)
                    {
                        try
                        {
                            //1.先读取资料

                            String ppgString = serialPort1.ReadLine();
                            String[] ppgArray2 = ppgString.Split(':');

                            //2.按照开头字段做判断
                            /*
                              未來的格式：
                              SPO2:xx.xxx
                              HR:xx.xxxx
                              TEMP:xx.xxxx
                              BME:xx.xx,xx.xx,xx.xx
                              BME_EN:xx.xx,xx.xx,xx.xx*/
                            switch (ppgArray2[0])
                            {
                                case "SPO2":
                                    ps.spo2 = ppgArray2[1];
                                    b_SPO2 = true;
                                    break;
                                case "HR":
                                    ps.hr = ppgArray2[1];
                                    HR = true;
                                    break;
                                case "TEMP":

                                    break;
                                case "BME":
                                    String[] ppgArray3 = ppgArray2[1].Split(',');
                                    ps.hand_temp = ppgArray3[0];
                                    ps.hand_humidity = ppgArray3[1];
                                    ps.hand_pressure = ppgArray3[2];
                                    b_BME = true;
                                    ps.envir_temp = "1";
                                    ps.envir_humidity = "2";
                                    ps.envir_pressure = "3";
                                    b_BME_EM = true;
                                    break;
                                case "BME_EN":

                                    break;

                            }

                        }
                        catch
                        {

                        }
                    }
                    Form4 form4 = new Form4();
                    form4.ShowDialog();
                    ps.ear_temp = temp.ToString();
                    records.Add(ps);
                    //using (var stream = File.Open(".\\data\\data.csv", FileMode.Append))
                    using (var stream = File.Open(this.tx_filepath.Text, FileMode.Append))
                    using (var writer = new StreamWriter(stream))
                    using (var csv = new CsvWriter(writer, config))
                    {
                        csv.WriteRecords(records);
                        MessageBox.Show("储存完毕");
                        serialPort1.Close();
                    }
                    BindCsv(tx_filepath.Text);
                }
            }
            else
            {
                if(this.tx_filepath.Text == "")
                    MessageBox.Show("請選擇文件");
                else
                     MessageBox.Show("請填寫基本信息");
                serialPort1.Close();
            }
        
        }


        private void button4_Click(object sender, EventArgs e)
        {
           
            var records = new List<PPG>
            {
                new PPG{ ppg1 = 1, ppg2 = 2, ppg3 = 8},
        
            };

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var stream = File.Open(".\\data\\ppg2.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(records);
                MessageBox.Show("储存完毕");
            }
        }
      
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            tx_filepath.Text = openFileDialog1.FileName;
            BindCsv(tx_filepath.Text);
        }

        private void BindCsv(string filepath)
        {
            DataTable dt = new DataTable();

            string[] rows = System.IO.File.ReadAllLines(filepath ,System.Text.Encoding.Default);
            if (rows.Length > 0)
            {
                string firstrow = rows[0];
                String[] features = firstrow.Split(',');
                foreach (string f in features)
                {
                    dt.Columns.Add(new DataColumn(f));
                }

                for (int i = 1; i <rows.Length; i++)
                {
                    string[] datawords = rows[i].Split(',');
                    DataRow dr = dt.NewRow();
                    int col = 0;
                    foreach (string f in features) 
                    {

                        dr[f] = datawords[col++];
                    }
                    dt.Rows.Add(dr);
                }
             }
                dataGridView1.DataSource = dt;

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
