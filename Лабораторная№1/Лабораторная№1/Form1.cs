﻿using System;
using System.Management;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Reflection;
using System.IO.Ports;

namespace Лабораторная_1
{
    public partial class App : Form
    {
        List<string> ports_l=new List<string>();
        public App()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
                   
            string Host = Dns.GetHostName();
            string IP = Dns.GetHostByName(Host).AddressList[0].ToString();
            string OS = Environment.OSVersion.ToString();

            this.IP.Text = IP;
            this.OS.Text = OS;
            

            GetRAM();
            GetDomain();
            GetHD();
            GetAntivirus();
            GetPorts();


            /*for (int i = 0; i < this.ports.Items.Count; i++)
            {
                ports_l[i]= this.ports.Items[i] + "\n";
            }*/

        }

        private void GetRAM()
        {
            ManagementObjectSearcher ramMonitor =    //запрос к WMI для получения памяти ПК
           new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (ManagementObject objram in ramMonitor.Get())
            {
                ulong totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);    //общая память ОЗУ
                                                                                        // ulong busyRam = totalRam - Convert.ToUInt64(objram["FreePhysicalMemory"]);         //занятная память = (total-free)
                                                                                        //Console.WriteLine(((busyRam * 100) / totalRam));       //вычисляем проценты занятой памяти
                RAM.Text = (totalRam / 1000 / 1024).ToString() + " Гб";
            }
        }

        private void GetDomain()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            Assembly[] assemblies = domain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                string s = asm.GetName().Name + "\r\n";
                dom_obj.Text = dom_obj.Text + s;
            }

            this.domain.Text = domain.FriendlyName;
        }

        private void GetHD()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                string s = "Имя диска:" + drive.Name + "\r\n" + "Объем диска:" + (drive.TotalSize / 1000 / 1024).ToString() + " Гб" + "\r\n";
                HD.Text = HD.Text + s;
            }
        }

        private void GetAntivirus()
        {
            string antv = "";
            ManagementObjectSearcher av_srch = new ManagementObjectSearcher("root\\SecurityCenter2","SELECT * FROM AntiVirusProduct");

            foreach (ManagementObject av in av_srch.Get())
            {
                try
                {
                    antv += av["dispayName"].ToString() + "\n";
                }

                catch
                {
                    antv = "Не найдено";
                }
                
            }
            antivirus.Text = antv;

        }

        void GetPorts()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnections = properties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation port in tcpConnections)
            {
                this.ports.Items.Add(port.LocalEndPoint.Port);
                ports_l.Add(port.LocalEndPoint.Port.ToString());
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Label[] labels = new Label[] { this.IP, this.OS, this.RAM, this.HD, this.domain, dom_obj,antivirus};
            List<string> ports=ports_l;
                        
            Отчет report = new Отчет(labels, ports_l);
            report.FormClosed += report_FormClosed;
            report.Show();
            this.Hide();
        }

        private void report_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show(); //отображение 1-й формы после закрытия 2-й
            this.Close();
        }

    }
}
