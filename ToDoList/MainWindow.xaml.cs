using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro;
using System.IO;
using System.Linq;

namespace MaximovInkToDoApp
{

    public partial class MainWindow
    {
        private static Color dark;
        private static Color light;
        private static Color accent;
        private static FontFamily font;
        //Checkboxs containter
        public static ListContainer container;
        //Label name todolist
        public TextBox label;
        //Add Checkbox button
        public Button button_add;

        public static MainWindow window;

        public MainWindow()
        {
            InitializeComponent();
            ShowMinButton = false;
            window = this;
            ShowMaxRestoreButton = false;
            ThemeManager_IsThemeChanged(null, null);
            MinWidth = 210;
            MinHeight = 100;
            Height = MinHeight;
            Width = MinWidth;
            font = FontFamily;
            GridContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            GridContent.VerticalAlignment = VerticalAlignment.Stretch;
            ThemesButton.Click += OpenThemesEdit;
            Create.Click += NewList;
            SaveButton.Click += Save;
            LoadButton.Click += Load;
            ThemeManager.IsThemeChanged += ThemeManager_IsThemeChanged;
            Closing += OnWindowClosing;
        }

        private void ThemeManager_IsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            accent = (Color)ThemeManager.DetectAppStyle().Item2.Resources["AccentColor"];
            light = Colors.Blend(accent, Color.FromRgb(120, 120, 120), 0.9);
            dark = Colors.Blend(accent, Color.FromRgb(20, 20,20), -0.9);

            GridContent.Background = new SolidColorBrush(accent);
            Background = new SolidColorBrush(accent);
            if (label != null)
                label.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (button_add != null)
                button_add.Background = new SolidColorBrush(dark);

            if (container != null)
                container.Background = new SolidColorBrush(light);

        }

        public void NewList(object sender, RoutedEventArgs args)
        {
            Background = new SolidColorBrush(Color.FromRgb(0, 200, 200));
            label = new TextBox();
            label.FontFamily = font;
            container = new ListContainer();
            label.Text = "NewToDoList";
            label.Background = new SolidColorBrush(Color.FromRgb(0, 200, 200));
            label.BorderThickness = new Thickness(0);
            AllowDrop = true;

            button_add = new Button();
            button_add.HorizontalAlignment = HorizontalAlignment.Stretch;
            button_add.VerticalAlignment = VerticalAlignment.Bottom;
            button_add.BorderThickness = new Thickness(0);
            button_add.Background = new SolidColorBrush(accent);
            button_add.Content = "Add";
            button_add.Click += b_Add;
            button_add.FontFamily = font;
            GridContent.Height = container.Height;
            GridContent.Children.Add(label);
            GridContent.Children.Add(container);
            GridContent.Children.Add(button_add);
            GridContent.UpdateLayout();
            ThemeManager_IsThemeChanged(null, null);
        }

        private void OpenThemesEdit(object sender, RoutedEventArgs e)
        {
            if (ThemesWindow.window != null)
                return;
            ThemesWindow themesWindow = new ThemesWindow();
            themesWindow.Show();
        }

        public class ListContainer : ListBox
        {
            public ObservableCollection<EditableCheckBox> _empList = new ObservableCollection<EditableCheckBox>();

            public ListContainer() : base()
            {
                BorderThickness = new Thickness(0);
                MinHeight = 40;
                ItemsSource = _empList;
                var style = new Style(typeof(ListBoxItem));
                style.Setters.Add(new Setter(AllowDropProperty, true)); Style itemContainerStyle = new Style(typeof(ListBoxItem));
                itemContainerStyle.Setters.Add(new Setter(AllowDropProperty, true));
                itemContainerStyle.Setters.Add(new EventSetter(PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(s_PreviewMouseRightButtonDownEvent)));
                itemContainerStyle.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(listbox1_Drop)));
                ItemContainerStyle = itemContainerStyle;
                AllowDrop = true;
                window.UpdateWindowSize();
            }

            void s_PreviewMouseRightButtonDownEvent(object sender, MouseButtonEventArgs e)
            {
                if (sender is ListBoxItem)
                {
                    ListBoxItem draggedItem = sender as ListBoxItem;
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }

            private void listbox1_Drop(object sender, DragEventArgs e)
            {
                if (sender is ListBoxItem)
                {
                    EditableCheckBox droppedData = e.Data.GetData(typeof(EditableCheckBox)) as EditableCheckBox;

                    EditableCheckBox target = ((ListBoxItem)(sender)).DataContext as EditableCheckBox;
                    int removedIdx = Items.IndexOf(droppedData);
                    int targetIdx = Items.IndexOf(target);

                    if (removedIdx < targetIdx)
                    {
                        _empList.Insert(targetIdx + 1, droppedData);
                        _empList.RemoveAt(removedIdx);
                    }
                    else
                    {
                        int remIdx = removedIdx + 1;
                        if (_empList.Count + 1 > remIdx)
                        {
                            _empList.Insert(targetIdx, droppedData);
                            _empList.RemoveAt(remIdx);
                        }
                    }
                }
            }
        }

        public void UpdateWindowSize()
        {
            double b_a = 0;
            double c_a = 0;

            if (container == null)
            {
                c_a = 40;
            }
            else if (container._empList.Count == 0)
                c_a = 50;
            else
            {
                //Debug.WriteLine(container.ActualHeight);
                c_a = container.ActualHeight;
            }

            if (button_add != null)
                b_a = button_add.ActualHeight;

            Height = 95 + Toolbar.ActualHeight + b_a + c_a;
        }

        public class EditableCheckBox : DockPanel
        {
            public CheckBox checkBox = new CheckBox();
            public TextBox textBox = new TextBox();
            public Button remove_button = new Button();
            public int index = -1;

            public EditableCheckBox(string content) : base()
            {
                textBox.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                textBox.BorderThickness = new Thickness(0);
                textBox.Text = content;
                textBox.AllowDrop = false;
                remove_button.Content = "-";
                remove_button.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                remove_button.Click += Destroy;
                Children.Add(checkBox);
                Children.Add(textBox);
                Children.Add(remove_button);
                window.UpdateWindowSize();
            }

            public EditableCheckBox(string content, bool value) : this(content)
            {
                checkBox.IsChecked = value;
                window.UpdateWindowSize();
            }

            public void Destroy(object sender, RoutedEventArgs e)
            {
                container._empList.Remove(this);
            }
        }

        private void b_Add(object sender, RoutedEventArgs e)
        {
            Add("CheckBox " + container._empList.Count);
        }

        private void Add(string name)
        {
            var check = new EditableCheckBox(name);
            TextBox textBox = new TextBox();
            check.textBox.FontFamily = font;
            container._empList.Add(check);
        }

        private void Add(string name, bool value)
        {
            var check = new EditableCheckBox(name, value);
            check.textBox.FontFamily = font;
            container._empList.Add(check);
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            if (container == null || container._empList.Count == 0)
                return;
            List<EditableCheckBox> editableCheckBoxes = container._empList.ToList();
            string text = label.Text + "/";



            for (int i = 0; i < editableCheckBoxes.Count; i++)
            {
                text += editableCheckBoxes[i].textBox.Text + "=" + (bool)editableCheckBoxes[i].checkBox.IsChecked + "|";
            }

            File.WriteAllText("todo.txt", text);
        }

        public void Load(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("todo.txt"))
                return;

            if (container == null)
                NewList(null,null);
            else
                container._empList.Clear();
            string text = File.ReadAllText("todo.txt");

            string[] split_name = text.Split('/');
            string[] parts = split_name[1].Split('|');
            label.Text = split_name[0];

            for (int i = 0; i < parts.Length-1; i++)
            {
                string[] key_value = parts[i].Split('=');
                Debug.WriteLine("key is " + key_value[0]);
                Debug.WriteLine("value is " + key_value[1]);
                Add(key_value[0], bool.Parse(key_value[1]));
            }

        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (ThemesWindow.window != null)
                ThemesWindow.window.Close();
        }

        private void GitHubClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/MaximovInk/WpfToDoList");
        }
    }

    public static class Colors
    {
        public static Color Blend(this Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromRgb(r, g, b);
        }
    }
}