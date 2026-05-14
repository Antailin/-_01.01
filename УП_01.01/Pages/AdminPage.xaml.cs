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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            LoadComplaints();
        }

        private void ShowOnly(StackPanel panel)
        {
            PanelComplaints.Visibility = Visibility.Collapsed;
            PanelUnfreeze.Visibility = Visibility.Collapsed;
            PanelRoles.Visibility = Visibility.Collapsed;
            PanelUsers.Visibility = Visibility.Collapsed;
            panel.Visibility = Visibility.Visible;
        }

        private void LoadComplaints()
        {
            Core.ResetContext();
            var list = Core.Context.Complaints.ToList()
                .Select(c => new
                {
                    ComplaintId = c.ComplaintId,
                    From = "От: " + (c.Users?.Login ?? ""),
                    Target = c.BookId != null
                        ? "Книга: " + c.Books?.Title
                        : "Отзыв #" + c.ReviewId,
                    Reason = c.Reason
                }).ToList();

            ComplaintsList.ItemsSource = list;
        }

        private void LoadUnfreezeRequests()
        {
            Core.ResetContext();
            var list = Core.Context.UnfreezeRequests
                .Where(ur => ur.Status == "Обработка")
                .ToList()
                .Select(ur => new
                {
                    RequestId = ur.UnfreezeRequestId,
                    UserLogin = ur.Users?.Login ?? "",
                    Reason = ur.Reason
                }).ToList();

            UnfreezeList.ItemsSource = list;
        }

        private void LoadRoleRequests()
        {
            Core.ResetContext();
            var list = Core.Context.RoleRequests
                .Where(rr => rr.Status == "Обработка")
                .ToList()
                .Select(rr => new
                {
                    RequestId = rr.RoleRequestId,
                    UserLogin = rr.Users?.Login ?? ""
                }).ToList();

            RoleRequestsList.ItemsSource = list;
        }

        private void LoadUsers()
        {
            Core.ResetContext();
            var list = Core.Context.Users.ToList()
                .Select(u => new
                {
                    UserId = u.UserId,
                    Login = u.Login,
                    RoleName = u.Roles?.RoleName ?? "",
                    FreezeLabel = u.IsFrozen == true
                        ? "Разморозить"
                        : "Заморозить"
                }).ToList();

            UsersList.ItemsSource = list;
        }

        private void BtnTabComplaints_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly(PanelComplaints);
            LoadComplaints();
        }

        private void BtnTabUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly(PanelUnfreeze);
            LoadUnfreezeRequests();
        }

        private void BtnTabRoles_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly(PanelRoles);
            LoadRoleRequests();
        }

        private void BtnTabUsers_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly(PanelUsers);
            LoadUsers();
        }

        private void BtnDeleteComplaint_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int id = (int)btn.Tag;

            var c = Core.Context.Complaints
                .FirstOrDefault(x => x.ComplaintId == id);
            if (c != null)
            {
                Core.Context.Complaints.Remove(c);
                Core.Context.SaveChanges();
                LoadComplaints();
            }
        }

        private void BtnApproveUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int id = (int)btn.Tag;

            var req = Core.Context.UnfreezeRequests
                .FirstOrDefault(ur => ur.UnfreezeRequestId == id);
            if (req == null) return;

            req.Status = "Одобрено";

            if (req.BookId != null)
            {
                var book = Core.Context.Books
                    .FirstOrDefault(b => b.BookId == req.BookId);
                if (book != null) book.IsFrozen = false;
            }
            else
            {
                var user = Core.Context.Users
                    .FirstOrDefault(u => u.UserId == req.UserId);
                if (user != null) user.IsFrozen = false;
            }

            Core.Context.SaveChanges();
            LoadUnfreezeRequests();
        }

        private void BtnRejectUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int id = (int)btn.Tag;

            var req = Core.Context.UnfreezeRequests
                .FirstOrDefault(ur => ur.UnfreezeRequestId == id);
            if (req != null)
            {
                req.Status = "Отклонено";
                Core.Context.SaveChanges();
                LoadUnfreezeRequests();
            }
        }

        private void BtnApproveRole_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int id = (int)btn.Tag;

            var req = Core.Context.RoleRequests
                .FirstOrDefault(rr => rr.RoleRequestId == id);
            if (req == null) return;

            req.Status = "Одобрено";

            var user = Core.Context.Users
                .FirstOrDefault(u => u.UserId == req.UserId);
            if (user != null) user.RoleId = 2;

            Core.Context.SaveChanges();
            LoadRoleRequests();
        }

        private void BtnRejectRole_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int id = (int)btn.Tag;

            var req = Core.Context.RoleRequests
                .FirstOrDefault(rr => rr.RoleRequestId == id);
            if (req != null)
            {
                req.Status = "Отклонено";
                Core.Context.SaveChanges();
                LoadRoleRequests();
            }
        }

        private void BtnToggleFreeze_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int id = (int)btn.Tag;

            var user = Core.Context.Users
                .FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                user.IsFrozen = !user.IsFrozen;
                Core.Context.SaveChanges();
                LoadUsers();
            }
        }
    }
}

