﻿using System;
using System.Collections.Generic;
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

using Microsoft.Win32; //追加
using System.IO;
using System.Collections.ObjectModel;

using System.Diagnostics;
using GraphVizWrapper.Queries;
using GraphVizWrapper;
using System.Drawing;
using GraphVizWrapper.Commands;
using Path = System.IO.Path;

namespace AirlinkToDot
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            bool exitOption = false;
            FileInfo buiFileInfo = null;

            if (App.CommandLineArgs != null)
            {
                if (GetBuiFileAndOption(ref buiFileInfo, ref exitOption))
                {
                    BuiFile buiFile = new BuiFile();
                    buiFile.Load(buiFileInfo.FullName);
                    convertBuiToDot(buiFile);
                    if (exitOption) Application.Current.Shutdown();//終了
                }

            }
        }


        /// <summary>
        /// Buiファイル名、/Nオプションを取得する
        /// </summary>
        /// <param name="buiFile"></param>
        /// <param name="exitOption"></param>
        private static bool GetBuiFileAndOption(ref FileInfo buiFileInfo, ref bool exitOption)
        {
            foreach (var str in App.CommandLineArgs)
            {
                if (str.StartsWith("/"))
                {
                    if (str.ToUpper().Contains("/N"))
                    {
                        exitOption = true;
                        continue;
                    }
                    // Error!
                    var msg = $"An unknown option, {str} is specified.";
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }


                var finfo = new FileInfo(str);
                if (finfo.Exists)
                {
                    buiFileInfo = finfo;
                }
                else
                {
                    // コマンドラインで指定されたファイルは存在しない。
                    // The file specified on the command line does not exist
                    MessageBox.Show($"The file, {finfo.Name} specified on the command line does not exist");
                    break;
                }
            }

            if (buiFileInfo != null) return true;
            else return false;

        }

        private void btnLoadBui_Click(object sender, RoutedEventArgs e)
        {
            BuiFile buiFile = new BuiFile();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.DefaultExt = "*.b*";
            ofd.Filter = "TRNBuld File (*.bui;*.b17;*.b18)|*.bui;*.b17;*.b18";
            if (ofd.ShowDialog() == true)
            {
                buiFile.Load(ofd.FileName);
                convertBuiToDot(buiFile);
            }
        }

        /// <summary>
        /// Bui to Dot
        /// </summary>
        /// <param name="buiFile">BuiFile class</param>
        private void convertBuiToDot(BuiFile buiFile)
        {
            var dir = System.IO.Path.GetDirectoryName(buiFile.FileName);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(buiFile.FileName);
            

            var baseFileName =  $@"{dir}\{fileName}_airnetwork.bui";



            var gvFileName = System.IO.Path.ChangeExtension(baseFileName, ".gv");
            bool append = false; // overwrite

            // convert bui to gv
            var strList = buiToDot(buiFile);

            // display 
            string gv = "";
            foreach (string str in strList)
            {
                gv += str + "\n";
            }
            this.txtBox.Text = gv;

            // save the .gv file
            MessageBoxResult ret;
            if (File.Exists(gvFileName))
            {
                this.Activate(); // Put this window forward
                var msg = "The default GV file already exists. Overwrite it?";
                ret = MessageBox.Show(msg, "Confirm saving default file", MessageBoxButton.YesNo);
                if (ret == MessageBoxResult.Yes)
                {
                    // Yes, Overwrite the existing file.
                    SaveGVFile(gvFileName, append, strList);
                }
                else
                {
                    // No, new filename is needed.
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = System.IO.Path.GetDirectoryName(gvFileName);
                    dlg.FileName = System.IO.Path.GetFileName(gvFileName); // Default file name
                    dlg.DefaultExt = ".gv"; // Default file extension
                    dlg.Filter = "Graphviz DOT File (.gv)|*.gv"; // Filter files by extension

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        gvFileName = dlg.FileName;
                        SaveGVFile(gvFileName, append, strList);

                    }
                }
            }
            else
            {
                SaveGVFile(gvFileName, append, strList);
            }

            bool pngRet = false;
            var pngFileName = System.IO.Path.ChangeExtension(gvFileName, ".png");
            if (File.Exists(pngFileName))
            {
                this.Activate(); // Put this window forward
                var msg = "The default PNG file already exists. Overwrite it?";
                ret = MessageBox.Show(msg, "Confirm saving default file", MessageBoxButton.YesNo);
                if (ret == MessageBoxResult.Yes)
                {
                    // Yes, Overwrite the existing file.
                    pngRet = SavePngFile(gv, pngFileName);
                }
                else
                {
                    // No, new filename is needed.
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = System.IO.Path.GetDirectoryName(pngFileName);
                    dlg.FileName = System.IO.Path.GetFileName(pngFileName); // Default file name
                    dlg.DefaultExt = ".png"; // Default file extension
                    dlg.Filter = "PNG (.png)|*.png"; // Filter files by extension

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        pngRet = SavePngFile(gv, dlg.FileName);
                    }
                }
            }
            else
            {
                // generate a image using Graphviz/Dot
                pngRet = SavePngFile(gv, pngFileName);
            }

            // Launch application associated with png file
            if(pngRet) Process.Start(pngFileName);

        }


        /// <summary>
        /// generate a image using Graphviz/Dot
        /// </summary>
        /// <param name="gv">data for GraphViz</param>
        /// <param name="pngFileName">PNG filename </param>
        /// <returns></returns>
        private static bool SavePngFile(string gv, string pngFileName)
        {
            try
            {
                generateGraphImage(gv, pngFileName);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                var msg = @"Graphviz executable program can not be found." + "\n";
                msg += @"Please copy the GraphViz program to the installation folder of this utility program." + "\n";
                msg += @"'C:\Program Files (x86)\TRNSYS.JP\AirlinkToDot\GraphViz'" + "\n";
                msg += "\n";
                msg += @"For details, refer to 'ReadMe.txt' in the installation folder.";
                MessageBox.Show(msg);
                return false;
            }
            return true;
        }

        private static void SaveGVFile(string filename, bool append, List<string> strList)
        {
            using (StreamWriter writer = new StreamWriter(filename, append, Encoding.GetEncoding("shift_jis")))　//Shift_JIS
            {
                foreach (string str in strList)
                {
                    writer.WriteLine(str);
                }
            }
        }


        /// <summary>
        /// convert Bui to Dot format
        /// </summary>
        /// <param name="buiFile"></param>
        /// <returns></returns>
        private static List<string> buiToDot(BuiFile buiFile)
        {
            List<string> strList = new List<string>();

            strList.Add("digraph {");
            strList.Add("    rankdir=LR;");

            List<string> zones = new List<string>();
            List<string> extNodes = new List<string>();
            List<string> auxNodes = new List<string>();
            List<string> constPressureNodes = new List<string>();


            // 数字で始まるZone名への対策
            // Zone名の1文字目が数字なら先頭に"_"を追加する
            foreach (Link link in buiFile.LinkList.Links)
            {
                int num = 0;
                if (int.TryParse(link.FromNode.Substring(0, 1), out num))
                {
                    link.FromNode = "_" + link.FromNode;
                }
                if (int.TryParse(link.ToNode.Substring(0, 1), out num))
                {
                    link.ToNode = "_" + link.ToNode;
                }
            }

            // ノードをZone, Ext, Auxへ分類する
            foreach (Link link in buiFile.LinkList.Links)
            {
                string node;
                node = link.FromNode;
                parseNode(zones, extNodes, auxNodes, constPressureNodes, node);
                node = link.ToNode;
                parseNode(zones, extNodes, auxNodes, constPressureNodes, node);
            }


            string zoneList = "";
            foreach (var str in zones) { zoneList += str + " "; }
            string extNodeList = "";
            foreach (var str in extNodes) { extNodeList += str + " "; }
            string auxNodeList = "";
            foreach (var str in auxNodes) { auxNodeList += str + " "; }
            string constPressureNodeList = "";
            foreach (var str in constPressureNodes) { constPressureNodeList += str + " "; }


            strList.Add("    node [shape = doublecircle]; " + zoneList + (string.IsNullOrEmpty(zoneList) ? "" : ";"));
            strList.Add("    node [shape = circle]; " + extNodeList + (string.IsNullOrEmpty(extNodeList) ? "" : ";"));
            strList.Add("    node [shape = rectangle]; " + auxNodeList + (string.IsNullOrEmpty(auxNodeList) ? "" : ";"));
            strList.Add("    node [style=rounded, shape = diamond]; " + constPressureNodeList + (string.IsNullOrEmpty(constPressureNodeList) ? "" : ";"));

            foreach (Link link in buiFile.LinkList.Links)
            {
                string str = $"    {link.FromNode} -> {link.ToNode}[ label = \"{link.LinkType} [{link.ID}]\" ];";
                strList.Add(str);
            }
            strList.Add("}  ");

            return strList;
        }

        /// <summary>
        /// generate an image from .gv strings
        /// </summary>
        /// <param name="diagraph"></param>
        /// <param name="imgFile"></param>
        private static void generateGraphImage(string diagraph, string imgFile)
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuerty = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuerty, getStartProcessQuery);

            var wrapper = new GraphGeneration(
                getStartProcessQuery,
                getProcessStartInfoQuerty,
                registerLayoutPluginCommand);

            // wrapper.RenderingEngine = Enums.RenderingEngine.Fdp;
            //wrapper.RenderingEngine = Enums.RenderingEngine.Sfdp;
            wrapper.RenderingEngine = Enums.RenderingEngine.Dot;

            byte[] output = wrapper.GenerateGraph(diagraph, Enums.GraphReturnType.Png);

            // byte[] to Image
            System.Drawing.Image img = byteArrayToImage(output);

            // save the image
            img.Save(imgFile, System.Drawing.Imaging.ImageFormat.Png);

            img.Dispose();
        }

        /// <summary>
        /// convert byte[] to an image
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static System.Drawing.Image byteArrayToImage(byte[] b)
        {
            ImageConverter imgconv = new System.Drawing.ImageConverter();
            System.Drawing.Image img = (System.Drawing.Image)imgconv.ConvertFrom(b);
            return img;
        }

        /// <summary>
        /// Parse the node type 
        /// </summary>
        /// <param name="zones"></param>
        /// <param name="extNodes"></param>
        /// <param name="auxNodes"></param>
        /// <param name="cpNodes"></param>
        /// <param name="node"></param>
        private static void parseNode(List<string> zones, List<string> extNodes, List<string> auxNodes, List<string> cpNodes, string node)
        {
            if (node.StartsWith("EN_"))
            {
                if (extNodes.Where((c) => c.Equals(node)).Count() < 1) extNodes.Add(node);
                return;
            }
            else if (node.StartsWith("AN_"))
            {
                if (auxNodes.Where((c) => c.Equals(node)).Count() < 1) auxNodes.Add(node);
                return;
            }
            else if (node.StartsWith("P_"))
            {
                if (cpNodes.Where((c) => c.Equals(node)).Count() < 1) cpNodes.Add(node);
                return;
            }
            else if (zones.IndexOf(node) < 0)
            {
                zones.Add(node);
            }
        }


        /// <summary>
        /// Version Info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutDlg aboutDlg = new AboutDlg();
            aboutDlg.Owner = this;
            aboutDlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            aboutDlg.ShowDialog();
        }

        /// <summary>
        /// Exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

        /// <summary>
        /// Drag&Drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Drop(object sender, DragEventArgs e)
        {
            DroppedFiles list = this.DataContext as DroppedFiles;
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                if (checkFileNmae(files[0]))
                {
                    BuiFile buiFile = new BuiFile();
                    buiFile.Load(files[0]);
                    convertBuiToDot(buiFile);
                }
                else
                {
                    this.Activate(); //  bring the window to the foreground.
                    var msg = "Non-bui file was dropped. Please drop the Bui file here.";
                    MessageBox.Show(this, msg, "Non-bui file", MessageBoxButton.OK);
                    this.txtBox.Text = "Error !!\n" + msg;

                }
            }

        }

        /// <summary>
        /// 拡張子がBuiファイルに合致するか判定する
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private bool checkFileNmae(string v)
        {
            var ext = Path.GetExtension(v).ToUpper();
            if (ext == ".BUI" || ext == ".B17" || ext == ".B18") return true;
            else return false;
        }

        public class DroppedFiles
        {
            public DroppedFiles()
            {
                FileNames = new ObservableCollection<string>();
            }
            public ObservableCollection<string> FileNames
            {
                get;
                private set;
            }
        }

        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

    }
}
