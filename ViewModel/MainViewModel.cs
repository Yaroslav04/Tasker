using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Tasker.Core;
using Tasker.Model;

namespace Tasker.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        public MainViewModel()
        {
            FileManager.FileInit();
            Items = new ObservableCollection<TasksSoket>();
            CompleteItems = new ObservableCollection<TasksSoket>();
            TaskItems = new ObservableCollection<TasksSoket>();
            ComplaintItems = new ObservableCollection<TasksSoket>();
            CategoriesAdd = new List<string> { "Задание", "Уголовные дела", "Заседания", "ОРД", "Жалобы", "Другое" };
            LoadItemsAsync();
            NotificationRun();

        }


        #region Properties

        public ObservableCollection<TasksSoket> Items { get; set; }

        public ObservableCollection<TasksSoket> CompleteItems { get; set; }

        public ObservableCollection<TasksSoket> TaskItems { get; set; }

        public ObservableCollection<TasksSoket> ComplaintItems { get; set; }

        public List<string> CategoriesAdd { get; set; }

        private string header;
        public string Header
        {
            get => header;
            set => SetProperty(ref header, value);
        }

        private string category;
        public string Category
        {
            get => category;
            set => SetProperty(ref category, value);
        }

        private string status;
        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        private Brush itemColor;
        public Brush ItemColor
        {
            get => itemColor;
            set => SetProperty(ref itemColor, value);
        }

        private TasksSoket selectedItem;
        public TasksSoket SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
            }
        }

        private int tabItem = 0;
        public int TabItem
        {
            get => tabItem;
            set
            {
                SetProperty(ref tabItem, value);
                if (tabItem != 4)
                {
                    ClearAddTab();
                }
                selectedItem = null;
            }
        }

        private int idAdd;
        public int IdAdd
        {
            get => idAdd;
            set => SetProperty(ref idAdd, value);
        }


        private string titleAdd;
        public string TitleAdd
        {
            get => titleAdd;
            set => SetProperty(ref titleAdd, value);
        }

        private string headerAdd;
        public string HeaderAdd
        {
            get => headerAdd;
            set => SetProperty(ref headerAdd, value);
        }

        private string categoryAdd = "Завдання";
        public string CategoryAdd
        {
            get => categoryAdd;
            set => SetProperty(ref categoryAdd, value);
        }

        private string statusAdd;
        public string StatusAdd
        {
            get => statusAdd;
            set => SetProperty(ref statusAdd, value);
        }

        private string dateAdd = DateTime.Now.ToShortDateString();
        public string DateAdd
        {
            get => dateAdd;
            set => SetProperty(ref dateAdd, value);
        }

        private string AddStatus;

        private string PreviousAddDate;

        #endregion

        #region Commands

        private ICommand command;
        public ICommand Command
        {
            get
            {
                return command ?? (command = new CommandHandler(param => CommandAction(param), true));
            }
        }

        public void CommandAction(object _parpam)
        {
            if (_parpam != null)
            {
                Debug.WriteLine(_parpam);
                switch (_parpam.ToString())
                {
                    case "send":
                        SendMail();
                        break;

                    case "saveCSV":
                        SaveToCSV();
                        break;

                    case "exit":
                        Exit();
                        break;

                    case "add":
                        OpenAdd();
                        break;

                    case "change":
                        Change();
                        break;

                    case "delete":
                        Delete();
                        break;

                    case "complete":
                        Complete();
                        break;

                    case "clone":
                        Clone();
                        break;

                    case "commandAdd":
                        CommandAdd();
                        break;
                }
            }
        }

        private void Clone()
        {
            if (SelectedItem != null)
            {
                DateAdd = SelectedItem.DateSoket;
                PreviousAddDate = SelectedItem.DateSoket;
                CategoryAdd = SelectedItem.Category;
                HeaderAdd = SelectedItem.Header;
                IdAdd = SelectedItem.Id;
                TitleAdd = "Клонировать";
                TabItem = 4;
                AddStatus = "clone";
            }
        }

        private async void Complete()
        {
            if (SelectedItem != null)
            {
                await App.DataBase.UpdateAsync(new Tasks
                {
                    Id = SelectedItem.Id,
                    Category = SelectedItem.Category,
                    Header = SelectedItem.Header,
                    Date = Convert.ToDateTime(SelectedItem.DateSoket),
                    Status = "complete"
                });
                await LoadItemsAsync();
            }
        }

        private async void Delete()
        {
            if (SelectedItem != null)
            {
                await App.DataBase.DeleteAsync(SelectedItem.Id);
                await LoadItemsAsync();
            }
        }

        private void Change()
        {
            if (SelectedItem != null)
            {
                DateAdd = SelectedItem.DateSoket;
                CategoryAdd = SelectedItem.Category;
                HeaderAdd = SelectedItem.Header;
                IdAdd = SelectedItem.Id;
                TitleAdd = "Изменить";
                TabItem = 4;
                AddStatus = "change";
            }
        }

        private async void CommandAdd()
        {
            if (AddStatus == "add")
            {
                await App.DataBase.SaveAsync(new Tasks
                {
                    Date = Convert.ToDateTime(DateAdd),
                    Category = CategoryAdd,
                    Header = HeaderAdd,
                    Status = "active"
                });

                TabItem = 0;
                await LoadItemsAsync();
            }
            else if (AddStatus == "change")
            {
                await App.DataBase.UpdateAsync(new Tasks
                {
                    Id = IdAdd,
                    Date = Convert.ToDateTime(DateAdd),
                    Category = CategoryAdd,
                    Header = HeaderAdd,
                    Status = "active"
                });

                IdAdd = -1;
                TabItem = 0;
                await LoadItemsAsync();
            }
            else if (AddStatus == "clone")
            {
                if (PreviousAddDate != DateAdd)
                {
                    await App.DataBase.UpdateAsync(new Tasks
                    {
                        Id = IdAdd,
                        Date = Convert.ToDateTime(PreviousAddDate),
                        Category = CategoryAdd,
                        Header = HeaderAdd,
                        Status = "complete"
                    });

                    await App.DataBase.SaveAsync(new Tasks
                    {
                        Date = Convert.ToDateTime(DateAdd),
                        Category = CategoryAdd,
                        Header = HeaderAdd,
                        Status = "active"
                    });

                    IdAdd = -1;
                    TabItem = 0;
                    await LoadItemsAsync();
                }
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void OpenAdd()
        {
            TabItem = 4;
            TitleAdd = "Сохранить";
            CategoryAdd = "Задание";
            AddStatus = "add";
        }

        private void ClearAddTab()
        {
            DateAdd = DateTime.Now.ToShortDateString();
            CategoryAdd = "Завдання";
            HeaderAdd = "";
        }

        #endregion

        #region Functions

        public async Task LoadItemsAsync()
        {
            await LoadItems();
        }

        public async Task LoadItems()
        {
            await ItemsInit();
            await ItemsTaskInit();
            await ItemsCompleteInit();
            await ItemsComplaintInit();
        }

        void NotificationRun()
        {
            Task.Run(() => NotificationAsync());
        }

        async void NotificationAsync()
        {
            while (true)
            {
                if (DateTime.Now.Hour > 16)
                {
                    await Task.Delay(30000);
                    if (PlayTrigger())
                    {
                        Play();
                        MessageBox.Show("Не выполнено задание/жалоба");
                    }

                }
                else
                {
                    await Task.Delay(1800000);
                    if (PlayTrigger())
                    {
                        Play();
                        MessageBox.Show("Не выполнено задание/жалоба");
                    }
                }
            }
        }

        void Play()
        {
            SoundPlayer player = new SoundPlayer("mixkit-happy-bells-notification-937.wav");
            player.Load();
            player.Play();
        }

        bool PlayTrigger()
        {
            bool sw = false;

            if (TaskItems.Count > 0)
            {
                foreach (var item in TaskItems)
                {
                    if (DateTime.Now >= item.Date)
                    {
                        sw = true;
                        break;
                    }
                }
            }

            if (ComplaintItems.Count > 0)
            {
                foreach (var item in TaskItems)
                {
                    if (DateTime.Now >= item.Date)
                    {
                        sw = true;
                        break;
                    }
                }
            }

            return sw;
        }

        public async Task ItemsInit()
        {
            Items.Clear();
            var _items = await App.DataBase.GetAsync();
            _items = _items.Where(x => x.Status == "active").ToList();
            _items = _items.OrderBy(x => x.Date).ToList();

            foreach (var item in _items)
            {
                TasksSoket _tasksSoket = new TasksSoket();
                _tasksSoket.Id = item.Id;
                _tasksSoket.Header = item.Header;
                _tasksSoket.Category = item.Category;
                _tasksSoket.Status = item.Status;
                _tasksSoket.DateSoket = item.Date.ToShortDateString();

                if ((item.Date - DateTime.Now).TotalDays < 3)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.IndianRed);
                }
                else if (DateTime.Now > item.Date)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.IndianRed);
                }
                else if ((item.Date - DateTime.Now).TotalDays > 7)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.Goldenrod);
                }

                Items.Add(_tasksSoket);
            }
        }

        public async Task ItemsCompleteInit()
        {
            CompleteItems.Clear();
            var _items = await App.DataBase.GetAsync();
            _items = _items.Where(x => x.Status == "complete").ToList();
            _items = _items.OrderByDescending(x => x.Date).ToList();

            foreach (var item in _items)
            {
                TasksSoket _tasksSoket = new TasksSoket();
                _tasksSoket.Id = item.Id;
                _tasksSoket.Header = item.Header;
                _tasksSoket.Category = item.Category;
                _tasksSoket.Status = item.Status;
                _tasksSoket.DateSoket = item.Date.ToShortDateString();

                CompleteItems.Add(_tasksSoket);
            }
        }
        public async Task ItemsTaskInit()
        {
            TaskItems.Clear();
            var _items = await App.DataBase.GetAsync();
            _items = _items.Where(x => x.Status == "active").ToList();
            _items = _items.Where(_x => _x.Category == "Задание").ToList();
            _items = _items.OrderBy(x => x.Date).ToList();

            foreach (var item in _items)
            {
                TasksSoket _tasksSoket = new TasksSoket();
                _tasksSoket.Id = item.Id;
                _tasksSoket.Header = item.Header;
                _tasksSoket.Category = item.Category;
                _tasksSoket.Status = item.Status;
                _tasksSoket.DateSoket = item.Date.ToShortDateString();

                if ((item.Date - DateTime.Now).TotalDays < 3)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.IndianRed);
                }
                else if (DateTime.Now > item.Date)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.IndianRed);
                }
                else if ((item.Date - DateTime.Now).TotalDays > 7)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.Goldenrod);
                }              

                TaskItems.Add(_tasksSoket);
            }
        }

        public async Task ItemsComplaintInit()
        {
            ComplaintItems.Clear();
            var _items = await App.DataBase.GetAsync();
            _items = _items.Where(x => x.Status == "active").ToList();
            _items = _items.Where(_x => _x.Category == "Жалобы").ToList();
            _items = _items.OrderBy(x => x.Date).ToList();

            foreach (var item in _items)
            {
                TasksSoket _tasksSoket = new TasksSoket();
                _tasksSoket.Id = item.Id;
                _tasksSoket.Header = item.Header;
                _tasksSoket.Category = item.Category;
                _tasksSoket.Status = item.Status;
                _tasksSoket.DateSoket = item.Date.ToShortDateString();

                if ((item.Date - DateTime.Now).TotalDays < 3)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.IndianRed);
                }
                else if (DateTime.Now > item.Date)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.IndianRed);
                }
                else if ((item.Date - DateTime.Now).TotalDays > 7)
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    _tasksSoket.Color = new SolidColorBrush(Colors.Goldenrod);
                }

                ComplaintItems.Add(_tasksSoket);
            }
        }

        public async Task SendMail()
        {
            if (Items.Count > 0)
            {
                await LoadItems();
                string _text = "";
                foreach (var item in Items)
                {
                    _text = _text + $"{item.DateSoket} {item.Category}\n{item.Header}\n\n";
                }
                await MailServise.SendEmailAsync(_text);
            }
        }

        public async Task SaveToCSV()
        {
            var _items = await App.DataBase.GetAsync();
            _items = _items.OrderBy(x => x.Date).ToList();

            if (_items.Count > 0)
            {
                await LoadItems();
                using (StreamWriter sw = new StreamWriter(FileManager.GeneralPath("data.csv")))
                {
                    foreach (var item in _items)
                    {
                        sw.WriteLine($"{item.Date.ToShortDateString()}\t{item.Header}\t{item.Category}\t{item.Status}");
                    }
                }
            }
        }

        #endregion   
    }
}
