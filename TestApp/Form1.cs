using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDB;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XDB.XDBConfigurator.SetConnection("Server=185.136.164.166;;Port=3306; Database=fuathoca_development;Uid=zertach;Pwd=ex060708******;Connect Timeout=30;Pooling=True;CharSet=utf8;");
            XDB.XDBConfigurator.Init(XDB.LoadType.FOLDER, "fuathoca_development");
            XDBConfigurator.UseCache = true;
            XDBConfigurator.CacheTimeout = 3;

            var result = XDB.Main.GetData<List<User>>("get_users", null, (error) =>
            {
                if (error.Error == -1)
                {
                    Console.WriteLine("no data found");
                }
                else
                {
                    Console.WriteLine("Error No : " + error.Error + " Description : ", error.Description);
                }
            });

            MessageBox.Show(result.Count.ToString());
        }
    }
}
