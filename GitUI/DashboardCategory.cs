﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using GitCommands;

namespace GitUI
{
    public partial class DashboardCategory : UserControl
    {
        public DashboardCategory()
        {
            InitializeComponent();
        }

        public DashboardCategory(string title, RepositoryCategory repositoryCategory)
        {
            InitializeComponent();

            this.Title = title;
            this.RepositoryCategory = repositoryCategory;
        }

        public void DisableContextMenu()
        {
            Caption.ContextMenuStrip = null;
        }

        private RepositoryCategory repositoryCategory;
        public RepositoryCategory RepositoryCategory
        {
            get
            {
                return repositoryCategory;
            }
            set
            {
                repositoryCategory = value;

                if (repositoryCategory != null && repositoryCategory.CategoryType == RepositoryCategoryType.RssFeed)
                    repositoryCategory.DownloadRssFeed();

                InitRepositoryCategory();

                if (repositoryCategory != null && repositoryCategory.Repositories != null)
                    repositoryCategory.ListChanged += new ListChangedEventHandler(Repositories_ListChanged);
            }
        }

        private void InitRepositoryCategory()
        {
            if (repositoryCategory != null)
            {
                foreach (Repository repository in repositoryCategory.Repositories)
                {
                    DashboardItem dashboardItem = new DashboardItem(repository);
                    dashboardItem.Click += new EventHandler(dashboardItem_Click);
                    AddItem(dashboardItem);

                    if (repositoryCategory.CategoryType == RepositoryCategoryType.Repositories)
                    {
                        ContextMenuStrip contextMenu = new ContextMenuStrip();
                        ToolStripMenuItem moveToMenuItem = new ToolStripMenuItem("Move To Category", null, new ToolStripMenuItem("moveto"));
                        moveToMenuItem.Tag = repository;
                        moveToMenuItem.DropDownOpening += new EventHandler(moveToMenuItem_DropDownOpening);
                        contextMenu.Items.Add(moveToMenuItem);
                        ToolStripMenuItem moveUpMenuItem = new ToolStripMenuItem("Move Up");
                        moveUpMenuItem.Tag = repository;
                        moveUpMenuItem.Click += new EventHandler(moveUpMenuItem_Click);
                        contextMenu.Items.Add(moveUpMenuItem);
                        ToolStripMenuItem moveDownMenuItem = new ToolStripMenuItem("Move Down");
                        moveDownMenuItem.Tag = repository;
                        moveDownMenuItem.Click += new EventHandler(moveDownMenuItem_Click);
                        contextMenu.Items.Add(moveDownMenuItem);
                        ToolStripMenuItem removeMenuItem = new ToolStripMenuItem("Remove");
                        removeMenuItem.Tag = repository;
                        removeMenuItem.Click += new EventHandler(removeMenuItem_Click);
                        contextMenu.Items.Add(removeMenuItem);
                        ToolStripMenuItem editMenuItem = new ToolStripMenuItem("Edit");
                        editMenuItem.Click += new EventHandler(editMenuItem_Click);
                        contextMenu.Items.Add(editMenuItem);

                        dashboardItem.ContextMenuStrip = contextMenu;
                    }
                }
            }
        }

        void moveUpMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem toolStripItem = sender as ToolStripItem;

            if (toolStripItem == null)
                return;

            Repository repository = toolStripItem.Tag as Repository;

            if (repository == null)
                return;

            int index = RepositoryCategory.Repositories.IndexOf(repository);
            RepositoryCategory.Repositories.Remove(repository);
            RepositoryCategory.Repositories.Insert(Math.Max(index-1, 0), repository);
        }

        void moveDownMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem toolStripItem = sender as ToolStripItem;

            if (toolStripItem == null)
                return;

            Repository repository = toolStripItem.Tag as Repository;

            if (repository == null)
                return;

            int index = RepositoryCategory.Repositories.IndexOf(repository);
            RepositoryCategory.Repositories.Remove(repository);
            RepositoryCategory.Repositories.Insert(Math.Min(index+1, RepositoryCategory.Repositories.Count), repository);
        }

        void editMenuItem_Click(object sender, EventArgs e)
        {
            new FormDashboardEditor().ShowDialog();
        }

        void Repositories_ListChanged(object sender, ListChangedEventArgs e)
        {
            Title = RepositoryCategory.Description;
            Clear();
            InitRepositoryCategory();
        }

        void removeMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem toolStripItem = sender as ToolStripItem;

            if (toolStripItem == null)
                return;

            Repository repository = toolStripItem.Tag as Repository;

            if (repository == null)
                return;

            RepositoryCategory.RemoveRepository(repository);

            InitRepositoryCategory();
        }

        void moveToMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem moveToMenuItem = (ToolStripMenuItem)sender;

            moveToMenuItem.DropDownItems.Clear();

            foreach (RepositoryCategory repositoryCategory in Repositories.RepositoryCategories)
            {
                if (repositoryCategory.CategoryType == RepositoryCategoryType.Repositories)
                {
                    ToolStripItem addToItem = moveToMenuItem.DropDownItems.Add(repositoryCategory.Description);
                    addToItem.Tag = moveToMenuItem.Tag;
                    addToItem.Click += new EventHandler(addToItem_Click);
                }
            }

            if (moveToMenuItem.DropDownItems.Count > 0)
                moveToMenuItem.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem newCategoryMenuItem = new ToolStripMenuItem("New category");
            newCategoryMenuItem.Tag = moveToMenuItem.Tag;
            newCategoryMenuItem.Click += new EventHandler(newCategoryMenuItem_Click);
            moveToMenuItem.DropDownItems.Add(newCategoryMenuItem);

        }

        void newCategoryMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem toolStripItem = sender as ToolStripItem;

            if (toolStripItem == null)
                return;

            Repository repository = toolStripItem.Tag as Repository;

            if (repository == null)
                return;

            FormDashboardCategoryTitle formDashboardCategoryTitle = new FormDashboardCategoryTitle();
            formDashboardCategoryTitle.ShowDialog();

            if (string.IsNullOrEmpty(formDashboardCategoryTitle.GetTitle()))
                return;

            RepositoryCategory newRepositoryCategory = new RepositoryCategory(formDashboardCategoryTitle.GetTitle());

            RepositoryCategory.RemoveRepository(repository);
            newRepositoryCategory.AddRepository(repository);

            Repositories.RepositoryCategories.Add(newRepositoryCategory);
        }

        public event EventHandler DashboardItemClick;


        void dashboardItem_Click(object sender, EventArgs e)
        {
            if (DashboardItemClick != null)
                DashboardItemClick(sender, e);
        }

        public string Title
        {
            get
            {
                return Caption.Text;
            }
            set
            {
                Caption.Text = value;
            }
        }

        int top = 26;

        public void AddItem(DashboardItem dashboardItem)
        {
            dashboardItem.Location = new Point(10, top);
            dashboardItem.Width = this.Width - 13;
            this.Controls.Add(dashboardItem);
            top += dashboardItem.Height;
            this.Height = top;
        }

        public void Clear()
        {
            for (int i = Controls.Count; i > 0; i--)
            {
                if (Controls[i - 1] is DashboardItem)
                    Controls.RemoveAt(i - 1);
            }
            top = 26;
        }

        private void DashboardCategory_SizeChanged(object sender, EventArgs e)
        {
            for (int i = Controls.Count; i > 0; i--)
            {
                if (Controls[i - 1] is DashboardItem)
                    Controls[i - 1].Width = Width - 13;
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        void addToItem_Click(object sender, EventArgs e)
        {
            ToolStripItem toolStripItem = sender as ToolStripItem;
            
            if (toolStripItem == null)
                return;

            Repository repository = toolStripItem.Tag as Repository;

            if (repository == null)
                return;

            foreach(RepositoryCategory newRepositoryCategory in Repositories.RepositoryCategories)
            {
                if (newRepositoryCategory.Description.Equals(toolStripItem.Text))
                {
                    RepositoryCategory.RemoveRepository(repository);
                    repository.RepositoryType = RepositoryType.Repository;
                    newRepositoryCategory.AddRepository(repository);
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormDashboardEditor().ShowDialog();
        }

        private void removeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Repositories.RepositoryCategories.Remove(RepositoryCategory);
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = Repositories.RepositoryCategories.IndexOf(RepositoryCategory);
            Repositories.RepositoryCategories.Remove(RepositoryCategory);
            Repositories.RepositoryCategories.Insert(Math.Max(index - 1, 0), RepositoryCategory);
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = Repositories.RepositoryCategories.IndexOf(RepositoryCategory);
            Repositories.RepositoryCategories.Remove(RepositoryCategory);
            Repositories.RepositoryCategories.Insert(Math.Min(index + 1, Repositories.RepositoryCategories.Count), RepositoryCategory);
        }
    }
}