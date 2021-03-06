﻿using Hugo_LAND.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HugoLandEditeur.Presentation
{
    public partial class frmOpen : Form
    {
        private Monde m_currentworld;
        private readonly HugoLANDContext context;

        public frmOpen()
        {
            InitializeComponent();
            context = new HugoLANDContext();
            
        }

        public Monde CurrentWorld
        {
            get
            {
                return m_currentworld;
            }
            set
            {
                m_currentworld = value;
            }
        }

        private void frmOpen_Load(object sender, EventArgs e)
        {

            if (context.Mondes.Count() > 0)
            {
                context.Mondes.Load();
                mondeBindingSource.DataSource = context.Mondes.ToList();
                getCurrentWorld();
            }
            // Mettre erreur ici
        }

        private void btnNext_Click(object sender, EventArgs e)
        {

            mondeBindingSource.MoveNext();
            getCurrentWorld();

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            mondeBindingSource.MovePrevious();
            getCurrentWorld();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            m_currentworld = getCurrentWorld();
            this.DialogResult = DialogResult.OK;
            this.Close();
            //mondeBindingSource.Current();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private Monde getCurrentWorld()
        {
            return (Monde)mondeBindingSource.Current;
            //Monde currentWorld = (Monde)mondeBindingSource.Current;
            //m_Width = currentWorld.LimiteX;
            //m_Height = currentWorld.LimiteY;
            //m_Description = currentWorld.Description;
            //m_id = currentWorld.Id;
        }

        //private void Form1_Load(object sender, System.EventArgs e)
        //{
        //    categoryBindingSource.DataSource = dbContext.Categories.ToList();
        //}
    }
}
