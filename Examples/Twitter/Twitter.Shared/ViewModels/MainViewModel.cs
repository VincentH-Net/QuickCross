using System;
using QuickCross;
using Twitter.Shared.Models;
using System.Collections.ObjectModel;

namespace Twitter.Shared.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            TweetList = new ObservableCollection<Tweet>();
            OnTweetChanged();
            Text = "";
        }

        #region Data-bindable properties and commands
        public Tweet Tweet /* Two-way data-bindable property that calls custom code in OnTweetChanged() from setter, generated with propdb2c snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Tweet; } set { if (_Tweet != value) { _Tweet = value; RaisePropertyChanged(PROPERTYNAME_Tweet); OnTweetChanged(); } } } private Tweet _Tweet; public const string PROPERTYNAME_Tweet = "Tweet";
        public ObservableCollection<Tweet> TweetList /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _TweetList; } set { if (_TweetList != value) { _TweetList = value; RaisePropertyChanged(PROPERTYNAME_TweetList); UpdateTweetListHasItems(); } } } private ObservableCollection<Tweet> _TweetList; public const string PROPERTYNAME_TweetList = "TweetList";
        public bool TweetListHasItems /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _TweetListHasItems; } protected set { if (_TweetListHasItems != value) { _TweetListHasItems = value; RaisePropertyChanged(PROPERTYNAME_TweetListHasItems); } } } private bool _TweetListHasItems; public const string PROPERTYNAME_TweetListHasItems = "TweetListHasItems";
        protected void UpdateTweetListHasItems() /* Helper method generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { TweetListHasItems = _TweetList != null && _TweetList.Count > 0; }
        public string Text /* Two-way data-bindable property that calls custom code in OnTextChanged() from setter, generated with propdb2c snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Text; } set { if (_Text != value) { _Text = value; RaisePropertyChanged(PROPERTYNAME_Text); OnTextChanged(); } } } private string _Text; public const string PROPERTYNAME_Text = "Text";
        public int CharactersLeft /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _CharactersLeft; } set { if (_CharactersLeft != value) { _CharactersLeft = value; RaisePropertyChanged(PROPERTYNAME_CharactersLeft); } } } private int _CharactersLeft; public const string PROPERTYNAME_CharactersLeft = "CharactersLeft";
        public RelayCommand SendCommand /* Data-bindable command that calls Send(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_SendCommand == null) _SendCommand = new RelayCommand(Send); return _SendCommand; } } private RelayCommand _SendCommand; public const string COMMANDNAME_SendCommand = "SendCommand";
        public RelayCommand DeleteCommand /* Data-bindable command that calls Delete(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_DeleteCommand == null) _DeleteCommand = new RelayCommand(Delete); return _DeleteCommand; } } private RelayCommand _DeleteCommand; public const string COMMANDNAME_DeleteCommand = "DeleteCommand";
        #endregion

        private void OnTweetChanged()
        {
            DeleteCommand.IsEnabled = (Tweet != null);
        }

        private void Delete()
        {
            TweetList.Remove(Tweet);
            Tweet = null;
        }

        private void OnTextChanged()
        {
            CharactersLeft = 140 - Text.Length;
            SendCommand.IsEnabled = (Text.Length > 0 && CharactersLeft >= 0);
        }

        private void Send()
        {
            var newTweet = new Tweet { Text = this.Text, CreatedAt = DateTime.Now, UserName = "Me" };
            TweetList.Insert(0, newTweet);
            Tweet = newTweet;
            Text = "";
        }
        
    }
}

// Design-time data support
#if DEBUG
namespace Twitter.Shared.ViewModels.Design
{
    public class MainViewModelDesign : MainViewModel
    {
        public MainViewModelDesign()
        {
            Text = "Text for a new tweet";
            var now = DateTime.Now;
            TweetList.Insert(0, new Tweet
            {
                Text = "Creating a simple Twitter app for Android with MvvmQuickCross",
                UserName = "Me",
                CreatedAt = now.AddSeconds(-115)
            });
            TweetList.Insert(0, new Tweet
            {
                Text = "Created an Android solution with an application and a library project",
                UserName = "Me",
                CreatedAt = now.AddSeconds(-63)
            });
            TweetList.Insert(0, new Tweet
            {
                Text = "Added Tweet model class",
                UserName = "Me",
                CreatedAt = now.AddSeconds(-45)
            });
            TweetList.Insert(0, new Tweet
            {
                Text = "Added viewmodel properties and commands with code snippets",
                UserName = "Me",
                CreatedAt = now.AddSeconds(-25)
            });
            TweetList.Insert(0, new Tweet
            {
                Text = "Added some hardcoded design data fot the viewmodel",
                UserName = "Me",
                CreatedAt = now.AddSeconds(-1)
            });
        }
    }
}
#endif
