﻿using IdentityModel.Client;
using ProductivityTools.Meetings.ClientCaller;
using ProductivityTools.Meetings.CoreObjects;
using ProductivityTools.Meetings.WpfClient.Automapper;
using ProductivityTools.Meetings.WpfClient.Controls;
using ProductivityTools.Meetings.WpfClient.Controls.MeetingItem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ProductivityTools.Meetings.WpfClient
{
    public class MeetingsVM
    {
        public ObservableCollection<MeetingItemVM> Meetings { get; set; }
        public ObservableCollection<TreeNode> Tree { get; set; }

        public ICommand GetMeetingsCommand { get; }
        public ICommand NewMeetingCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand FilterMeetingsCommand { get; }
        public ICommand AddTreeNodeCommand { get; }
        public ICommand RemoveTreeNodeCommand { get; }
        public string Secret { get; set; }


        MeetingsClient client;
        private MeetingsClient Client
        {
            get
            {
                if (this.client == null)
                {
                    this.client = new MeetingsClient(this.Secret);
                }
                return this.client;
            }
        }
        TreeNode TreeNodeSelected { get; set; }

        public MeetingsVM()
        {
            this.Meetings = new ObservableCollection<MeetingItemVM>();
            this.Tree = new ObservableCollection<TreeNode>();
            this.GetMeetingsCommand = new CommandHandler(GetMeetings, () => true);
            this.NewMeetingCommand = new CommandHandler(NewMeeting, () => true);
            this.FilterMeetingsCommand = new CommandHandler(FilterMeeting, () => true);
            this.AddTreeNodeCommand = new CommandHandler(AddTreeNode, () => true);

            this.Meetings.Add(new MeetingItemVM(new CoreObjects.Meeting() { AfterNotes = "Core", BeforeNotes = "Core", DuringNotes = "Core", Subject = "fdsa" }));
            this.Meetings.Add(new MeetingItemVM(new CoreObjects.Meeting() { AfterNotes = "Core", BeforeNotes = "Core", DuringNotes = "Core" }));
            this.Tree.Add(new TreeNode("Pawel"));
            this.Tree.Add(new TreeNode("Marcin"));
        }

        private void AddTreeNode()
        {
            EditTreeNode edit = new EditTreeNode();
            edit.ShowDialog();
        }

        private async void FilterMeeting(object parameter)
        {

            var args = (RoutedPropertyChangedEventArgs<object>)parameter;
            if (args.NewValue == null) { return; }

            if (parameter != null)
            {
                TreeNode selectedItem = (TreeNode)args.NewValue;
                var xx = await Client.GetMeetings(selectedItem.Id);
                this.TreeNodeSelected = selectedItem;
                UpdateMeetings(xx);
            }
        }

        private async void GetMeetings()
        {
            MeetingsClient client = new MeetingsClient(this.Secret);
            try
            {
                var xx = await client.GetMeetings();
                var tree = await client.GetTree();
                this.Tree.Clear();
                tree.ForEach(x => this.Tree.Add(x));
                UpdateMeetings(xx);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void UpdateMeetings(List<Meeting> xx)
        {
            this.Meetings.Clear();
            foreach (var item in xx)
            {
                var meeting = new MeetingItemVM(item);
                this.Meetings.Add(meeting);
            }
        }

        private void NewMeeting()
        {
            var meeting = new CoreObjects.Meeting();
            meeting.TreeId = this.TreeNodeSelected.Id;
            meeting.Subject = this.TreeNodeSelected.Name;
            var meetingvm = new MeetingItemVM(meeting);
            this.Meetings.Add(meetingvm);
            EditMeeting edit = new EditMeeting(meeting);
            edit.Show();
        }
    }
}
