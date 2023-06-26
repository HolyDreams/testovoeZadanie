using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace testovoeZadanie
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

        string sqlQuery;
        List<StockStruct> mainList = new List<StockStruct>();
        private void butLogin_Click(object sender, RoutedEventArgs e)
        {
            sqlQuery = $@"SELECT login,
                                 password
                          FROM users
                          WHERE login = '{textLogin.Text}' AND
                                password = '{textPassword.Password}'";
            var res = SQLRequest.SQLite(sqlQuery);
            if (res.Rows.Count > 0)
            {
                panelLogin.Visibility = Visibility.Hidden;
                panelGrid.Visibility = Visibility.Visible;
                loadTable();
            }
        }
        private void loadTable()
        {
            sqlQuery = $@"SELECT stock.Name,
                                 CountObj,
                                 bom.Name AS bomName,
                                 Price,
                                 SumPrice,
                                 'Склад1' AS type
                          FROM Stock1 AS stock INNER JOIN
                                 BomSection AS bom ON stock.bom = bom.ID
                          UNION ALL
                          SELECt stock.Name,
                                 CountObj,
                                 bom.Name AS bomName,
                                 Price,
                                 SumPrice,
                                 'Склад2' AS type
                          FROM Stock2 AS stock INNER JOIN
                                 BomSection AS bom ON stock.bom = bom.ID";
            var res = SQLRequest.SQLite(sqlQuery);
            if (res.Rows.Count > 0)
            {
                var resList = (from DataRow a in res.Rows
                               select new StockStruct
                               {
                                   Name = (string)a["Name"],
                                   CountObj = double.Parse(a["CountObj"].ToString()),
                                   BomName = (string)a["bomName"],
                                   Price = double.Parse(a["Price"].ToString()),
                                   SumPrice = double.Parse(a["SumPrice"].ToString()),
                                   Type = (string)a["type"]
                               }).ToList();
                var sumList = resList.GroupBy(q => q.Name).Select(a => new StockStruct
                {
                    Name = a.Key,
                    CountObj = a.Sum(q => q.CountObj),
                    BomName = a.First().BomName,
                    Price = a.Average(q => q.Price),
                    SumPrice = a.Sum(q => q.SumPrice),
                    Type = "Общее кол-во"
                }).ToList();
                var obsCollection = new ObservableCollection<StockStruct>();
                mainList.AddRange(resList);
                mainList.AddRange(sumList);
                foreach (var item in mainList)
                    obsCollection.Add(item);

                ICollectionView colView = CollectionViewSource.GetDefaultView(obsCollection);

                colView.GroupDescriptions.Add(new PropertyGroupDescription("Type"));

                gridControl.DataContext = colView;
            }
        }

        private void butPrint_Click(object sender, RoutedEventArgs e)
        {
            labelPrintTime.Content = "Время печати  " + DateTime.Now.ToString();
            labelPrintTime.Visibility = Visibility.Visible;
            butPrint.Visibility = Visibility.Hidden;

            PrintDialog print = new PrintDialog();
            if ((bool)print.ShowDialog().GetValueOrDefault())
            {
                double columnWidths = 13;
                foreach (var col in gridControl.Columns)
                    columnWidths += col.ActualWidth;
                Size pageSize = new Size(print.PrintableAreaWidth > columnWidths ? columnWidths : print.PrintableAreaWidth, print.PrintableAreaHeight);
                panelGrid.Measure(pageSize);
                panelGrid.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));

                print.PrintVisual(panelGrid, Title);

                labelPrintTime.Visibility = Visibility.Hidden;
                butPrint.Visibility = Visibility.Visible;
            }
        }
    }
}