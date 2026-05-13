using System;
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

namespace УП_01._01.Pages
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private List<BookViewModel> _allBooks;

        public CatalogPage()
        {
            InitializeComponent();
            LoadGenres();
            LoadBooks();
        }

        private void LoadGenres()
        {
            var genres = Core.Context.Genres.ToList();

            CmbGenre.Items.Clear();
            CmbGenre.Items.Add("Все жанры");

            foreach (var g in genres)
                CmbGenre.Items.Add(g.Name);

            CmbGenre.SelectedIndex = 0;
        }

        private void LoadBooks()
        {
            var books = Core.Context.Books
                .Where(b => b.IsFrozen == false)
                .ToList();

            _allBooks = books.Select(b => new BookViewModel
            {
                BookId = b.BookId,
                Title = b.Title,
                DisplayName = b.Users?.DisplayName ?? "Неизвестен",
                AvgRating = b.Reviews.Any()
                              ? b.Reviews.Average(r => r.Rating)
                              : 0
            }).ToList();

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var result = _allBooks.AsEnumerable();

            string search = TxtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search) && search != "Поиск по названию или автору...")
            {
                search = search.ToLower();
                result = result.Where(b =>
                    b.Title.ToLower().Contains(search) ||
                    b.DisplayName.ToLower().Contains(search));
            }

            if (CmbGenre.SelectedIndex > 0)
            {
                string genre = CmbGenre.SelectedItem.ToString();
                var bookIds = Core.Context.BookGenre
                    .Where(bg => bg.Genres.Name == genre)
                    .Select(bg => bg.BookId)
                    .ToList();
                result = result.Where(b => bookIds.Contains(b.BookId));
            }

            if (CmbSort.SelectedIndex == 1)
                result = result.OrderByDescending(b => b.AvgRating);
            else
                result = result.OrderBy(b => b.Title);

            BooksPanel.ItemsSource = result.ToList();
        }

        private void BookCard_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var book = border?.DataContext as BookViewModel;
            if (book == null) return;

            NavigationService.Navigate(new BookPage(book.BookId));
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allBooks == null) return;
            ApplyFilters();
        }

        private void CmbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allBooks == null) return;
            ApplyFilters();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allBooks == null) return;
            ApplyFilters();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = "Поиск по названию или автору...";
            TxtSearch.Foreground = System.Windows.Media.Brushes.Gray;
            CmbGenre.SelectedIndex = 0;
            CmbSort.SelectedIndex = 0;
            ApplyFilters();
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == "Поиск по названию или автору...")
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtSearch.Text))
            {
                TxtSearch.Text = "Поиск по названию или автору...";
                TxtSearch.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }

    public class BookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public double AvgRating { get; set; }
    }
}
