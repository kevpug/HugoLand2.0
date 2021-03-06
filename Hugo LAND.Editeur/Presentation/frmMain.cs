﻿using Hugo_LAND.Core.Models;
using HugoLandEditeur.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity.Core.Objects;

namespace HugoLandEditeur
{
    public partial class frmMain : Form
    {

        private CMap m_Map;
        private Monde m_CurrentWorld;
        private CTileLibrary m_TileLibrary;
        private Dictionary<ObjetMonde, string> m_DObj;
        private Dictionary<Monstre, string> m_DMonstre;
        private Dictionary<Item, string> m_DItem;
        private List<Item> m_li;
        private List<ObjetMonde> m_OBJ;
        private List<Monstre> m_Mons;
        private int m_XSel;
        private int m_YSel;
        private int m_TilesHoriz;
        private System.Windows.Forms.Timer timer1;
        private int m_TilesVert;
        private bool m_bRefresh;
        private bool m_bResize;
        private int m_Zoom;
        private Rectangle m_TileRect;
        private Rectangle m_LibRect;
        private int m_ActiveXIndex;
        private bool m_worldNew;
        private int m_ActiveYIndex;
        private readonly frmLogin loginForm;
        private bool m_WorldOpen = false;

        internal void ConnectionReussie()
        {
            loginForm.Dispose();
            this.Enabled = true;
        }

        private int m_ActiveTileID;
        private int m_ActiveTileXIndex;
        private int m_ActiveTileYIndex;

        /// <summary>
        /// Summary description for Form1.
        /// </summary>
        /// 	
        public struct ComboItem
        {
            public string Text;
            public int Value;

            public ComboItem(string text, int val)
            {
                Text = text;
                Value = val;
            }
            public override string ToString()
            {
                return Text;
            }
        };

        public frmMain()
        {
            InitializeComponent();
            loginForm = new frmLogin(this);
            //DialogResult result;
            //bool bResult;


        }

        /* -------------------------------------------------------------- *\
        frmMain_Load()			
        - Main Form Initialization		
    \* -------------------------------------------------------------- */
        private void frmMain_Load(object sender, System.EventArgs e)
        {
            m_Map = new CMap();
            m_CurrentWorld = new Monde();
            m_TileLibrary = new CTileLibrary();
            m_Map.TileLibrary = m_TileLibrary;
            m_DObj = new Dictionary<ObjetMonde, string>();
            m_DMonstre = new Dictionary<Monstre, string>();
            m_DItem = new Dictionary<Item, string>();
            m_OBJ = new List<ObjetMonde>();
            m_li = new List<Item>();
            m_Mons = new List<Monstre>();
            picMap.Parent = picEditArea;
            picMap.Left = 0;
            picMap.Top = 0;

            picTiles.Parent = picEditSel;
            picTiles.Width = m_TileLibrary.Width * csteApplication.TILE_WIDTH_IN_IMAGE;
            picTiles.Height = m_TileLibrary.Height * csteApplication.TILE_HEIGHT_IN_IMAGE;
            picTiles.Left = 0;
            picTiles.Top = 0;

            vscMap.Minimum = 0;
            vscMap.Maximum = m_Map.Height;
            m_YSel = 0;

            hscMap.Minimum = 0;
            hscMap.Maximum = m_Map.Width;
            m_XSel = 0;

            m_bRefresh = true;
            m_bResize = true;
            timer1.Enabled = true;
            m_Zoom = csteApplication.ZOOM;

            m_TileRect = new Rectangle(-1, -1, -1, -1);
            m_LibRect = new Rectangle(-1, -1, -1, -1);
            m_ActiveTileID = 32;

            //dlgLoadMap.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\maps\\";
            //dlgSaveMap.InitialDirectory = dlgLoadMap.InitialDirectory;
            m_bOpen = false;
            m_MenuLogic();
            //tmrLoad.Enabled = true;	

            m_pen = new Pen(Color.Orange, 4);
            m_brush = new SolidBrush(Color.FromArgb(160, 249, 174, 55));
            m_brush2 = new SolidBrush(Color.FromArgb(160, 255, 0, 0));

            m_bDrawTileRect = false;
            m_bDrawMapRect = false;

            cboZoom.Left = 270;
            cboZoom.Top = 2;
            cboZoom.Items.Add(new ComboItem("1X", 1));
            cboZoom.Items.Add(new ComboItem("2X", 2));
            cboZoom.Items.Add(new ComboItem("4X", 4));
            cboZoom.Items.Add(new ComboItem("8X", 8));
            cboZoom.Items.Add(new ComboItem("16X", 16));
            cboZoom.SelectedIndex = 1;
            cboZoom.DropDownStyle = ComboBoxStyle.DropDownList;

            lblZoom.Left = 180;
            lblZoom.Top = 2;

            tbMain.Controls.Add(lblZoom);
            tbMain.Controls.Add(cboZoom);
        }



        /* -------------------------------------------------------------- *\
        Menus
    \* -------------------------------------------------------------- */
        #region Menu Code
        private void mnuFileExit_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void mnuHelpAbout_Click(object sender, System.EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog(this);
        }

        private void mnuZoomX1_Click(object sender, System.EventArgs e)
        {
            ResetScroll();
            m_Zoom = 1;
            m_bResize = true;
        }

        private void mnuZoomX2_Click(object sender, System.EventArgs e)
        {
            ResetScroll();
            m_Zoom = 2;
            m_bResize = true;
        }

        private void mnuZoomX4_Click(object sender, System.EventArgs e)
        {
            ResetScroll();
            m_Zoom = 4;
            m_bResize = true;
        }

        private void mnuZoomX8_Click(object sender, System.EventArgs e)
        {
            ResetScroll();
            m_Zoom = 8;
            m_bResize = true;
        }

        private void mnuZoomX16_Click(object sender, System.EventArgs e)
        {
            ResetScroll();
            m_Zoom = 16;
            m_bResize = true;
        }

        private void mnuFileOpen_Click(object sender, System.EventArgs e)
        {
            LoadMap();
        }

        private void mnuFileSave_Click(object sender, System.EventArgs e)
        {
            if (m_CurrentWorld != null)
            {
                m_SaveMap();
            }
        }

        private void tbMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            if (e.Button == tbbSave)
                m_SaveMap();
            if (e.Button == tbbOpen)
                LoadMap();
            else if (e.Button == tbbNew)
                NewMap();
        }

        #endregion


        /* -------------------------------------------------------------- *\
            vscMap_Scroll()
            - vertical scroll bar for map editor window		
        \* -------------------------------------------------------------- */
        private void vscMap_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            m_YSel = e.NewValue;
            if (m_bOpen)
                picMap.Refresh();
        }

        /* -------------------------------------------------------------- *\
            hscMap_Scroll()
            - horizontal scroll bar for map editor window		
        \* -------------------------------------------------------------- */
        private void hscMap_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            m_XSel = e.NewValue;
            if (m_bOpen)
                picMap.Refresh();
        }

        /* -------------------------------------------------------------- *\
            picEditArea_Resize()
			
            - resize event for the parent of the map. The edit area is
              auto-sized to the space not taken by the lower and right 
              panes.		
        \* -------------------------------------------------------------- */
        private void picEditArea_Resize(object sender, System.EventArgs e)
        {
            if (m_bOpen)
            {
                m_XSel = 0;
                hscMap.Value = m_XSel;
                m_YSel = 0;
                vscMap.Value = m_YSel;
                m_bResize = true;
            }
        }

        /* -------------------------------------------------------------- *\
            timer1_Tick()
			
            - I'm not sure if this is necessary or not, but I was having
              difficulty updating things correctly due to timing of resizing
              items or updating scrolls and their values not getting set 
              until after the event already occurred... so I'm setting
              flags instead.
        \* -------------------------------------------------------------- */
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (m_bRefresh)
            {
                m_bRefresh = false;
                picMap.Refresh();
            }
            if (m_bResize)
            {
                m_bResize = false;
                m_ResizeMap();
            }
            if (m_bRefreshLib)
            {
                m_bRefreshLib = false;
                picTiles.Refresh();
            }
        }

        /* -------------------------------------------------------------- *\
            picMap_Paint()
			
            - This is where the Map picture box is painted to.
              This event happens when Refresh() is called or any section
              of the picture box is invalidated (i.e. covering up part of
              the picture box with another windows and then moving it away)
        \* -------------------------------------------------------------- */
        private void picMap_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (m_bOpen)
            {
                if (m_XSel < 0)
                    m_XSel = 0;
                if (m_YSel < 0)
                    m_YSel = 0;
                m_Map.Draw(e.Graphics, e.ClipRectangle, m_XSel, m_YSel);

                if (m_bDrawMapRect)
                    e.Graphics.FillRectangle(m_brush, m_TileRect);
            }
        }

        /* -------------------------------------------------------------- *\
            m_ResizeMap()
			
            - Takes care of Zoom, scroll and visible area logic.
        \* -------------------------------------------------------------- */
        private void m_ResizeMap()
        {
            int xpos, ypos;
            int nWidth = (vscMap.Left - 0); //picEditArea.Left);
            int AvailableWidth = nWidth - (2 * csteApplication.BUFFER_WIDTH);
            m_TilesHoriz = AvailableWidth / (m_Zoom * csteApplication.TILE_WIDTH_IN_MAP);
            int nMapWidth = m_TilesHoriz * csteApplication.TILE_WIDTH_IN_MAP * m_Zoom;
            int BorderX = (nWidth - nMapWidth) / 2;

            int nHeight = (hscMap.Top - 0); //picEditArea.Top);
            int AvailableHeight = nHeight - 2 * csteApplication.BUFFER_HEIGHT;
            m_TilesVert = AvailableHeight / (m_Zoom * csteApplication.TILE_HEIGHT_IN_MAP);
            int nMapHeight = m_TilesVert * csteApplication.TILE_HEIGHT_IN_MAP * m_Zoom;
            int BorderY = (nHeight - nMapHeight) / 2;

            PrintDebug("AvailableHeight = " + AvailableHeight.ToString());
            PrintDebug("BorderY = " + BorderY.ToString());
            PrintDebug("AvailableWidth = " + AvailableWidth.ToString());
            PrintDebug("BorderX = " + BorderX.ToString());

            m_Map.OffsetX = 0; //BorderX;
            m_Map.OffsetY = 0; //BorderY;						
            m_Map.Zoom = m_Zoom;

            if (m_TilesHoriz < m_Map.Width)
            {
                //xpos = 16;
                xpos = 16 + (AvailableWidth - nMapWidth) / 2;
                m_Map.TilesHoriz = m_TilesHoriz;
                hscMap.Maximum = m_Map.Width - m_TilesHoriz;
            }
            else
            {
                m_Map.TilesHoriz = m_Map.Width;
                nMapWidth = m_Map.Width * csteApplication.TILE_WIDTH_IN_MAP * m_Zoom;
                xpos = 16 + (AvailableWidth - nMapWidth) / 2;
                hscMap.Maximum = 0;
            }

            if (m_TilesVert < m_Map.Height)
            {
                //ypos = 32;
                ypos = 32 + (AvailableHeight - nMapHeight) / 2;
                m_Map.TilesVert = m_TilesVert;
                vscMap.Maximum = m_Map.Height - m_TilesVert;
            }
            else
            {
                m_Map.TilesVert = m_Map.Height;
                nMapHeight = m_Map.Height * csteApplication.TILE_HEIGHT_IN_MAP * m_Zoom;
                ypos = 32 + (AvailableHeight - nMapHeight) / 2;
                vscMap.Maximum = 0;
            }

            picMap.Location = new System.Drawing.Point(xpos, ypos);
            picMap.Size = new Size(nMapWidth, nMapHeight);

            m_bRefresh = true;
        }


        /* -------------------------------------------------------------- *\
            picMap_MouseMove()
			
            - Keeps track / translates coordinates to map tile to be
              updated if clicked on.
        \* -------------------------------------------------------------- */
        private void picMap_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.X < 0 || e.Y < 0)
                return;
            if (e.X < m_TileRect.Left || e.X > m_TileRect.Right || e.Y < m_TileRect.Top || e.Y > m_TileRect.Bottom)
            {
                m_bDrawMapRect = true;

                m_Map.PointToTile(e.X, e.Y, ref m_ActiveXIndex, ref m_ActiveYIndex);
                m_Map.PointToBoundingRect(e.X, e.Y, ref m_TileRect);
                m_ActiveXIndex += m_XSel;
                m_ActiveYIndex += m_YSel;

                m_bRefresh = true;

                PrintDebug("XIndex = " + m_ActiveXIndex.ToString() + " YIndex = " + m_ActiveYIndex.ToString());
            }
        }

        /* -------------------------------------------------------------- *\
            picMap_Click()
			
            - Plots the ActiveTile from the tile library to the selected
              tile location on the map.
        \* -------------------------------------------------------------- */
        private void picMap_Click(object sender, System.EventArgs e)
        {
            //hUGO : mODIFIER ICI POUR AVOIR le tile et le type

            modificationMap(m_ActiveXIndex, m_ActiveYIndex, m_ActiveTileID);

            m_Map.PlotTile(m_ActiveXIndex, m_ActiveYIndex, m_ActiveTileID);
             //m_Obj.x = m_ActiveTileXIndex;
            //m_Obj.y = m_ActiveTileYIndex;
            //m_Obj.TypeObjet = m_ActiveTileID;
            //m_Obj.Monde = m_CurrentWorld;
            //m_Obj.Description = m_TileLibrary.ObjMonde.

            //m_CurrentWorld.ObjetMondes.Add(m_Obj);

            m_bRefresh = true;
        }

        /* -------------------------------------------------------------- *\
            picTiles_Paint()
			
            - Paints the tile library at the bottom of the screen.
        \* -------------------------------------------------------------- */
        private void picTiles_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (m_TileLibrary != null)
            {
                m_TileLibrary.Draw(e.Graphics, e.ClipRectangle);
                if (m_bDrawTileRect)
                    e.Graphics.FillRectangle(m_brush2, m_LibRect);
            }
        }

        /* -------------------------------------------------------------- *\
            picTiles_Click()
			
            - Selects the active tile ID
        \* -------------------------------------------------------------- */
        private void picTiles_Click(object sender, System.EventArgs e)
        {
            m_ActiveTileID = m_TileLibrary.TileToTileID(m_ActiveTileXIndex, m_ActiveTileYIndex);
            picActiveTile.Refresh();
        }

        /* -------------------------------------------------------------- *\
            vscTiles_Scroll()
			
            - controls the tile library scroll / position
        \* -------------------------------------------------------------- */
        private void vscTiles_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            picTiles.Top = -e.NewValue;
        }

        /* -------------------------------------------------------------- *\
            picTiles_MouseMove()
			
            - Keeps track / translates coordinates to tilelibrary tile to be
              selected if clicked on.
        \* -------------------------------------------------------------- */
        private void picTiles_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.X < 0 || e.Y < 0)
                return;
            if (e.X < m_LibRect.Left || e.X > m_LibRect.Right || e.Y < m_LibRect.Top || e.Y > m_LibRect.Bottom)
            {
                m_bDrawTileRect = true;

                m_TileLibrary.PointToTile(e.X, e.Y, ref m_ActiveTileXIndex, ref m_ActiveTileYIndex);
                m_TileLibrary.PointToBoundingRect(e.X, e.Y, ref m_LibRect);

                m_bRefreshLib = true;

                PrintDebug("TileXIndex = " + m_ActiveTileXIndex.ToString() + " TileYIndex = " + m_ActiveTileYIndex.ToString());
                PrintDebug("X = " + e.X.ToString() + " Y = " + e.Y.ToString());
            }
        }

        /* -------------------------------------------------------------- *\
            ResetScroll()
			
            - Resets the scrollbar to 0.
              I'm not sure if this is necessary anymore.. I was trouble-
              shooting an odd issue.			  
        \* -------------------------------------------------------------- */
        private void ResetScroll()
        {
            vscMap.Value = 0;
            m_YSel = 0;
            hscMap.Value = 0;
            m_XSel = 0;
        }

        /* -------------------------------------------------------------- *\
            picActiveTile_Paint()
			
            - Displays the selected tile.	  
        \* -------------------------------------------------------------- */
        private void picActiveTile_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Rectangle destrect = new Rectangle(0, 0, picActiveTile.Width, picActiveTile.Height);
            m_TileLibrary.DrawTile(e.Graphics, m_ActiveTileID, destrect);
        }

        /* -------------------------------------------------------------- *\
            tmrLoad_Tick()
			
            - Loads the default map. 
        \* -------------------------------------------------------------- */
        private void tmrLoad_Tick(object sender, System.EventArgs e)
        {
            tmrLoad.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            m_Map.Refresh();
            m_bOpen = true;
            m_bRefresh = true;
            picMap.Visible = true;
            m_MenuLogic();
            this.Cursor = Cursors.Default;
        }
        #region Debug Code

        private void PrintDebug(String strDebug)
        {
            Console.WriteLine(strDebug);
        }
        #endregion

        private void LoadMap()
        {

            frmOpen o;
            DialogResult result;
            bool bResult;
            m_DObj.Clear();
            m_DMonstre.Clear();
            m_DItem.Clear();
            m_OBJ.Clear();
            m_li.Clear();
            m_Mons.Clear();

            o = new frmOpen();

            result = o.ShowDialog(this);
            m_bOpen = false;
            picMap.Visible = false;
            this.Cursor = Cursors.WaitCursor;
            if (result == DialogResult.OK)
            {
                m_CurrentWorld = o.CurrentWorld;
                m_bOpen = false;
                picMap.Visible = false;
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    m_Map.Load(m_CurrentWorld);
                    FillLists();
                    m_bOpen = true;
                    m_bRefresh = true;
                    m_bResize = true;
                    picMap.Visible = true;
                    m_WorldOpen = true;
                    m_worldNew = false;
                }
                catch
                {
                    Console.WriteLine("Error Loading...");
                }
            }
            m_MenuLogic();
            this.Cursor = Cursors.Default;


            //DialogResult result;

            //dlgLoadMap.Title = "Load Map";
            //dlgLoadMap.Filter = "Map Files (*.map)|*.map|All Files (*.*)|*.*";

            //result = dlgLoadMap.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    m_bOpen = false;
            //    picMap.Visible = false;
            //    this.Cursor = Cursors.WaitCursor;
            //    try
            //    {
            //        m_Map.Load(dlgLoadMap.FileName);
            //        m_bOpen = true;
            //        m_bRefresh = true;
            //        picMap.Visible = true;
            //    }
            //    catch
            //    {
            //        Console.WriteLine("Error Loading...");
            //    }
            //    m_MenuLogic();
            //    this.Cursor = Cursors.Default;
            //}
        }

        /* -------------------------------------------------------------- *\
            m_SaveMap()
			
            - Saves the current map to the selected filename / path
        \* -------------------------------------------------------------- */
        private void m_SaveMap()
        {
            int idNewWorld = 0;
            HugoLANDContext context = new HugoLANDContext();
            
            if (m_worldNew)
            {
                MondeCRUD.CreerMonde(m_CurrentWorld);
                List<Monde> lm = MondeCRUD.ListeMonde();
                idNewWorld = lm.Last().Id;
                m_worldNew = false;
            }
            else
            {
                 idNewWorld = m_CurrentWorld.Id;
            }



            //Sauvegarde des 
            foreach (var om in m_DObj)
            {
                

                switch (om.Value)
                {
                    case "ORIGINAL":
                        continue; //Fait rien
                    case "NEW":
                        ObjetMondeCRUD.CreeObjetMonde(om.Key, idNewWorld);
                        break;
                    case "MODIFIED":
                        ObjetMondeCRUD.ChangeDescriptionObjetMonde(om.Key, idNewWorld);
                        break;
                }
                
            }
            foreach (var item in m_OBJ)
            {
                ObjetMondeCRUD.SupprimeObjetMonde(item, idNewWorld);
            }

            m_DObj.Clear();
            m_OBJ.Clear();
            

            foreach (var monstre in m_DMonstre)
                switch (monstre.Value)
                {
                    case "ORIGINAL":
                        continue; //Fait rien
                    case "NEW":
                        MonstreCRUD.CreerMonstre(monstre.Key, idNewWorld);
                        break;
                    case "MODIFIED":
                        MonstreCRUD.ModifierMonstre(monstre.Key, idNewWorld);
                        break;
                }
            foreach (var item in m_Mons)
            {
                MonstreCRUD.SupprimerMonstre(item);
            }

            m_Mons.Clear();
            m_DMonstre.Clear();

            foreach (var i in m_DItem)
                switch (i.Value)
                {
                    case "ORIGINAL":
                        continue; //Fait rien
                    case "NEW":
                        ItemCRUD.CreerItem(i.Key, idNewWorld);
                        break;
                    case "MODIFIED":
                       // ItemCRUD.(i.Key); // Revoir
                        break;
                }
            foreach (var item in m_li)
            {
                ItemCRUD.SupprimerItem(item);
            }

            m_li.Clear();
            m_DItem.Clear();

            FillLists();
            //m_Map.Save(m_CurrentWorld, m_WorldOpen);
            //DialogResult result;

            //dlgSaveMap.Title = "Save Map";
            //dlgSaveMap.Filter = "Map File (*.map)|*.map";

            //result = dlgSaveMap.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    this.Cursor = Cursors.WaitCursor;
            //    try
            //    {
            //        m_Map.Save(dlgSaveMap.FileName);
            //    }
            //    catch
            //    {
            //        Console.WriteLine("Error Saving...");
            //    }
            //    this.Cursor = Cursors.Default;
            //}
        }

        /* -------------------------------------------------------------- *\
            m_NewMap()
			
            - Creates a new map of the selected size.
        \* -------------------------------------------------------------- */
        private void NewMap()
        {
            frmNew f;
            DialogResult result;
            bool bResult;
            f = new frmNew();
            m_DObj.Clear();
            m_DMonstre.Clear();
            m_DItem.Clear();
            m_OBJ.Clear();
            m_li.Clear();
            m_Mons.Clear();
            m_CurrentWorld = null;
            m_CurrentWorld = new Monde();
            f.MapWidth = m_Map.Width;
            f.MapHeight = m_Map.Height;
            f.MapDescription = m_Map.Description;

            result = f.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                m_bOpen = false;
                picMap.Visible = false;
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    bResult = m_Map.CreateNew(f.MapWidth, f.MapHeight, f.MapDescription, 32);
                    m_CurrentWorld.LimiteX = m_Map.Width;
                    m_CurrentWorld.LimiteY = m_Map.Height;
                    m_CurrentWorld.Description = m_Map.Description;
                    m_worldNew = true;
                    if (bResult)
                    {
                        m_bOpen = true;
                        m_bRefresh = true;
                        m_bResize = true;
                        picMap.Visible = true;
                    }
                }
                catch
                {
                    Console.WriteLine("Error Creating...");
                }
                m_MenuLogic();
                this.Cursor = Cursors.Default;
            }
        }

        /* -------------------------------------------------------------- *\
            m_MenuLogic()
			
            - Enables / Disables menus based on application status
        \* -------------------------------------------------------------- */
        private void m_MenuLogic()
        {
            bool bEnabled;

            bEnabled = m_bOpen;
            mnuFileSave.Enabled = bEnabled;
            mnuFileClose.Enabled = bEnabled;
            //mnuManageCreateUser.Enabled = bEnabled;
            //mnuZoom.Enabled = bEnabled;
            tbbSave.Enabled = bEnabled;
        }

        /* -------------------------------------------------------------- *\
            mnuFileNew_Click()
        \* -------------------------------------------------------------- */
        private void mnuFileNew_Click(object sender, System.EventArgs e)
        {
            NewMap();
        }

        private void picTiles_MouseLeave(object sender, System.EventArgs e)
        {
            m_bDrawTileRect = false;
            m_LibRect.X = -1;
            m_LibRect.Y = -1;
            m_LibRect.Width = -1;
            m_LibRect.Height = -1;
            m_bRefreshLib = true;
        }

        private void picMap_MouseLeave(object sender, System.EventArgs e)
        {
            m_bDrawMapRect = false;
            m_TileRect.X = -1;
            m_TileRect.Y = -1;
            m_TileRect.Width = -1;
            m_TileRect.Height = -1;
            m_bRefresh = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            ComboItem myItem;
            myItem = (ComboItem)cboZoom.SelectedItem;
            ResetScroll();
            m_Zoom = myItem.Value;
            m_bResize = true;
            picTiles.Focus();
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            this.Enabled = false;
            loginForm.ShowDialog();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            //Create user...
            frmCreateUser f = new frmCreateUser();
            f.ShowDialog();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            //Create admin
            frmAdmin f = new frmAdmin();
            f.ShowDialog();
        }

        private void modificationMap(int x, int y, int tileID)
        {
            if (m_Map.getMapTileType(y, x) == tileID)
                return;


            Tile tileSelected = m_TileLibrary.ObjMonde.Values.Where(c => c.IndexTypeObjet == tileID).First();
            Tile tileUnder = m_TileLibrary.ObjMonde.Values.Where(c => c.IndexTypeObjet == m_Map.getMapTileType(y, x)).First();
            ObjetMonde objOriginal = m_DObj.Keys.ToList().Where(c => c.x == x && c.y == y).FirstOrDefault();
            Monstre monstreOriginal = m_DMonstre.Keys.ToList().Where(c => c.x == x && c.y == y).FirstOrDefault();
            Item itemOriginal = m_DItem.Keys.ToList().Where(c => c.x == x && c.y == y).FirstOrDefault();
            string testOriginality = "";

            if (objOriginal != null)
            {
                testOriginality = m_DObj[objOriginal];
            }
            else if (monstreOriginal != null)
            {
                testOriginality = m_DMonstre[monstreOriginal];
            }
            else if (itemOriginal != null)
            {
                testOriginality = m_DItem[itemOriginal];
            }

            if (tileSelected.IndexTypeObjet == 32)
            {
                if (testOriginality == "MODIFIED" || testOriginality == "ORIGINAL")
                {
                    if (tileUnder.TypeObjet == TypeTile.ObjetMonde)
                    {
                        m_OBJ.Add(m_DObj.Keys.Where(c => c.x == x && c.y == y).First());
                        m_DObj.Remove(m_DObj.Keys.Where(c => c.x == x && c.y == y).First());
                        return;
                    }
                    else if (tileUnder.TypeObjet == TypeTile.Item)
                    {
                        m_li.Add(m_DItem.Keys.Where(c => c.x == x && c.y == y).First());
                        m_DItem.Remove(m_DItem.Keys.Where(c => c.x == x && c.y == y).First());
                        return;
                    }
                    else
                    {
                        m_Mons.Add(m_DMonstre.Keys.Where(c => c.x == x && c.y == y).First());
                        m_DMonstre.Remove(m_DMonstre.Keys.Where(c => c.x == x && c.y == y).First());
                        return;
                    }
                }

                if (tileUnder.TypeObjet == TypeTile.ObjetMonde)
                {
                    m_DObj.Remove(objOriginal);
                    return;
                }
                else if (tileUnder.TypeObjet == TypeTile.Monstre)
                {
                    m_DMonstre.Remove(monstreOriginal);
                    return;
                }
                else
                {
                    m_DItem.Remove(itemOriginal);
                    return;
                }
            }


            if (tileSelected.TypeObjet == TypeTile.ObjetMonde && testOriginality == "ORIGINAL" || testOriginality == "MODIFIED")
            {
                m_DObj.Keys.Where(c => c.x == x && c.y == y).First().Description = tileSelected.Name;
                m_DObj.Keys.Where(c => c.x == x && c.y == y).First().TypeObjet = tileSelected.IndexTypeObjet;
                m_DObj[m_DObj.Keys.Where(c => c.x == x && c.y == y).First()] = "MODIFIED";
                return;
            }
            else if (tileSelected.TypeObjet == TypeTile.Item && (testOriginality == "ORIGINAL" || testOriginality == "MODIFIED") && (tileUnder.TypeObjet == TypeTile.Item))
            {
                m_DItem.Keys.Where(c => c.x == x && c.y == y).First().Description = tileSelected.Name;
                m_DItem.Keys.Where(c => c.x == x && c.y == y).First().ImageId = tileSelected.IndexTypeObjet;
                m_DItem.Keys.Where(c => c.x == x && c.y == y).First().Nom = tileSelected.Name;
                m_DItem[m_DItem.Keys.Where(c => c.x == x && c.y == y).First()] = "MODIFIED";
                return;
            }
            else if (tileSelected.TypeObjet == TypeTile.Monstre && (testOriginality == "ORIGINAL" || testOriginality == "MODIFIED") && (tileUnder.TypeObjet == TypeTile.Monstre))
            {
                m_DMonstre.Keys.Where(c => c.x == x && c.y == y).First().Nom = tileSelected.Name;
                m_DMonstre.Keys.Where(c => c.x == x && c.y == y).First().ImageId = tileSelected.IndexTypeObjet;
                m_DMonstre.Keys.Where(c => c.x == x && c.y == y).First().StatPV = tileSelected.Health;
                m_DMonstre[m_DMonstre.Keys.Where(c => c.x == x && c.y == y).First()] = "MODIFIED";
                return;
            }

            if (tileSelected.TypeObjet == TypeTile.ObjetMonde && (testOriginality != "ORIGINAL") && (testOriginality != "MODIFIED"))
            {
                if (tileUnder.IndexTypeObjet != 32 && testOriginality == "NEW")
                {
                    if (tileUnder.TypeObjet == TypeTile.Monstre)
                    {
                        m_DMonstre.Remove(monstreOriginal);
                    }
                    else if (tileUnder.TypeObjet == TypeTile.Item)
                    {
                        m_DItem.Remove(itemOriginal);
                    }
                    else
                    {
                        m_DObj.Remove(objOriginal);
                    }
                    m_DObj.Add(new ObjetMonde()
                    {
                        TypeObjet = tileSelected.IndexTypeObjet,
                        x = x,
                        y = y,
                        Monde = m_CurrentWorld,
                        Description = tileSelected.Name
                    },
                    "NEW");
                }
                else
                {
                    m_DObj.Add(new ObjetMonde()
                    {
                        TypeObjet = tileSelected.IndexTypeObjet,
                        x = x,
                        y = y,
                        Monde = m_CurrentWorld,
                        Description = tileSelected.Name
                    },
                    "NEW");
                }
                return;
            }

            if (tileSelected.TypeObjet == TypeTile.Monstre)
            {
                if (tileUnder.IndexTypeObjet != 32)
                {
                    if (tileUnder.TypeObjet == TypeTile.ObjetMonde && tileUnder.IsBlock)
                    {
                        m_DObj.Remove(objOriginal);
                    }
                    else if (tileUnder.TypeObjet == TypeTile.Item)
                    {
                        m_DItem.Remove(itemOriginal);
                    }
                    //else
                    //{
                    //    m_DMonstre.Remove(monstreOriginal);
                    //}
                    m_DMonstre.Add(new Monstre()
                    {
                        Nom = tileSelected.Name,
                        StatPV = tileSelected.Health,
                        Monde = m_CurrentWorld,
                        x = x,
                        y = y,
                        ImageId = tileSelected.IndexTypeObjet,
                        //À revoir juste en dessous !!!
                        StatDmgMax = 0,
                        StatDmgMin = 0,
                        Niveau = 0
                    },
                    "NEW");
                }
                else
                {
                    m_DMonstre.Add(new Monstre()
                    {
                        Nom = tileSelected.Name,
                        StatPV = tileSelected.Health,
                        Monde = m_CurrentWorld,
                        x = x,
                        y = y,
                        ImageId = tileSelected.IndexTypeObjet,
                        //À revoir juste en dessous !!!
                        StatDmgMax = 0,
                        StatDmgMin = 0,
                        Niveau = 0
                    },
                    "NEW");
                }
                return;
            }

            if (tileSelected.TypeObjet == TypeTile.Item)
            {
                if (tileUnder.IndexTypeObjet != 32)
                {
                    if (tileUnder.TypeObjet == TypeTile.Monstre)
                    {
                        m_DMonstre.Remove(monstreOriginal);
                    }
                    else if (tileUnder.TypeObjet == TypeTile.ObjetMonde && tileUnder.IsBlock)
                    {
                        m_DObj.Remove(objOriginal);
                    }
                    //else
                    //{
                    //    m_DItem.Remove(itemOriginal);
                    //}

                    m_DItem.Add(new Item()
                    {
                        ImageId = tileSelected.IndexTypeObjet,
                        x = x,
                        y = y,
                        Monde = m_CurrentWorld,
                        Description = tileSelected.Name,
                        Nom = tileSelected.Name
                    },
                    "NEW");
                }
                else
                {
                    m_DItem.Add(new Item()
                    {
                        ImageId = tileSelected.IndexTypeObjet,
                        x = x,
                        y = y,
                        Monde = m_CurrentWorld,
                        Description = tileSelected.Name,
                        Nom = tileSelected.Name
                    },
                    "NEW");
                }
                return;
            }
        }
            

        private void FillLists()
        {
            m_DObj.Clear();
            m_DMonstre.Clear();
            m_DItem.Clear();
            m_OBJ.Clear();
            m_li.Clear();
            m_Mons.Clear();
            // Remplir les listes des objects déjà existant.
            foreach (ObjetMonde om in m_CurrentWorld.ObjetMondes)
            {
                m_DObj.Add(
                    om,
                    "ORIGINAL"
                );
            }
            foreach (Item i in m_CurrentWorld.Items)
            {
                m_DItem.Add(
                    i,
                    "ORIGINAL"
                );
            }
            foreach (Monstre m in m_CurrentWorld.Monstres)
            {
                m_DMonstre.Add(
                    m,
                    "ORIGINAL"
                );
            }
        }

        private void mnuFileClose_Click(object sender, EventArgs e)
        {
            picMap.Visible = false;
            m_CurrentWorld = null;
            m_CurrentWorld = new Monde();
            m_DObj.Clear();
            m_DMonstre.Clear();
            m_DItem.Clear();
            m_OBJ.Clear();
            m_li.Clear();
            m_Mons.Clear();
        }
    }
}
