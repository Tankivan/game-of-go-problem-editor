using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace go_test_second
{
    public partial class Form1 : Form
    {
        int cols;
        int rows;
        public Form1(int c, int r)
        {
            cols = c;
            rows = r;
            InitializeComponent();
            saveFileDialog1.Filter = "Sgf файл(*.sgf)|*.sgf|All files(*.*)|*.*";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Первоначальная настройка Data
            
            Data.Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Data.Moves = new string[cols * rows * 2];
            Data.Design_Field = new int[rows, cols];
            Data.Field = new int[rows + 2, cols + 2];
            Data.Width = cols;
            Data.Height = rows;
            Data.Color = 'b';
            Data.Current_move = 0;
            FieldStartFill();
            Data.LastMoveB = CreateAndFill(Data.Width + 2, Data.Height + 2, Data.Field);
            Data.LastMoveW = CreateAndFill(Data.Width + 2, Data.Height + 2, Data.Field);
            // Создание игрового поля
            Point point = new Point() // Расположение
            {
                X = 0,
                Y = 0
            };
            Padding padding = new Padding() // Расстояние до края
            {
                All = 0
            };
            Size size_all = new Size() // Размер
            {
                Width = 32 * Data.Width,
                Height = 32 * Data.Height,
                
            };
            TableLayoutPanel tableLayoutPanel1 = new TableLayoutPanel() // Настройка
            {
                Location = point,
                ColumnCount = Data.Width + 1,
                RowCount = Data.Height + 1,
                Margin = padding,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Size = size_all
            };
            // Цикл, проходящий по каждой клетке таблицы
            for (int i = 0; i <= rows; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                for (int j = 0; j <= cols; j++)
                {
                    if (i == rows || j == cols)
                    {
                        Label label = new Label() // Заглушка, чтобы все столбцы были одинакового размера
                        {
                            Height = 32,
                            Width = 32,
                            Visible = true,
                            Text = ""
                        };
                        tableLayoutPanel1.Controls.Add(label);
                        continue;
                    }
                    // Создание PictureBox в клетке
                    Size size = new Size()
                    {
                        Width = 32,
                        Height = 32
                    };
                    ToolStripMenuItem delItem = new ToolStripMenuItem("Удалить")
                    {
                        Name = "delItem",
                        Enabled = false
                    };
                    delItem.Click += DelItemClick;
                    ToolStripMenuItem changeItem = new ToolStripMenuItem("Сменить цвет")
                    {
                        Name = "changeItem"
                    };
                    changeItem.Click += ChangeColor;
                    ContextMenuStrip menuStrip = new ContextMenuStrip();
                    menuStrip.Items.Add(delItem);
                    menuStrip.Items.Add(changeItem);
                    PictureBox picture = new PictureBox() // Добавление картинки в зависимости от расположения на доске
                    {
                        Size = size,
                        Margin = padding,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Dock = DockStyle.Fill,
                        Image = Properties.Resources.sprite,
                        ContextMenuStrip = menuStrip
                    };
                    picture.MouseEnter += Stone_hover;
                    picture.MouseLeave += Stone_unhover;
                    picture.MouseClick += PictureBox_Click;
                    // Добавление PictureBox в таблицу
                    tableLayoutPanel1.Controls.Add(picture);
                    Data.Design_Field[i, j] = 10;
                    if (i == 0)
                    {
                        Data.Design_Field[i, j] = 20;
                        picture.Image = Properties.Resources.top_edge;
                    }
                    if (i == rows - 1)
                    {
                        Data.Design_Field[i, j] = 30;
                        picture.Image = Properties.Resources.down_edge;
                    }
                    if (j == 0)
                    {
                        Data.Design_Field[i, j] = 50;
                        picture.Image = Properties.Resources.left_edge;
                    }
                    if (j == cols - 1)
                    {
                        Data.Design_Field[i, j] = 40;
                        picture.Image = Properties.Resources.right_edge;
                    }
                    if (i == 0 && j == 0)
                    {
                        Data.Design_Field[i, j] = 70;
                        picture.Image = Properties.Resources.left_top_corner;
                    }
                    if (i == rows - 1 && j == 0)
                    {
                        Data.Design_Field[i, j] = 90;
                        picture.Image = Properties.Resources.left_down_corner;
                    }
                    if (i == 0 && j == cols - 1)
                    {
                        Data.Design_Field[i, j] = 60;
                        picture.Image = Properties.Resources.right_top_corner;
                    }
                    if (i == rows - 1 && j == cols - 1)
                    {
                        Data.Design_Field[i, j] = 80;
                        picture.Image = Properties.Resources.right_down_corner;
                    }

                    if (j == 0)
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                }
            }

            
            // Добавление таблицы на форму
            tableLayoutPanel1.Name = "table_layout_panel1";
            tableLayoutPanel1.Resize += OnResize;
            this.Controls.Add(tableLayoutPanel1);
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel1);
            Size board_size = new Size()
            {
                Width = splitContainer1.ClientSize.Height,
                Height = splitContainer1.ClientSize.Height
            };
            splitContainer1.Size = board_size;
            splitContainer1.SplitterDistance = splitContainer1.Panel1.Height;
            splitContainer1.Margin = padding;
            splitContainer1.BorderStyle = BorderStyle.None;
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Height = splitContainer1.Panel1.Top;
            splitContainer1.SplitterDistance = tableLayoutPanel1.Height;
        }

        private void OnResize(object sender, EventArgs e)
        {
            TableLayoutPanel table = splitContainer1.Panel1.Controls["table_layout_panel1"] as TableLayoutPanel;
            table.Height = splitContainer1.Panel1.Height;
            table.Width = table.Height;
        }
        public void FieldStartFill()
        {
            for (int i = 0; i < Data.Height + 2; i++)
            {
                for (int j = 0; j < Data.Width + 2; j++)
                {
                    if (i == 0 || i == Data.Height + 1 || j == 0 || j == Data.Width + 1)
                    {
                        Data.Field[i, j] = 7;
                    }
                    else Data.Field[i, j] = 0;
                }
            }
        }

        public int[,] CreateAndFill(int cols, int rows, int[,] reference)
        {
            int[,] field = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    field[i, j] = reference[i, j];
                }
            }
            return field;
        }

        private void Current_position_button_click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count == 0)
            {
                TreeNode node = new TreeNode("Начальная позиция");
                treeView1.Nodes.Add(node);
                Node_Data nd = new Node_Data();
                nd.Field = Data.Field.Clone() as int[,];
                nd.Design_Field = Data.Design_Field.Clone() as int[,];
                nd.Comment = "";
                node.Tag = nd;
            }
        }

        private void DelItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip menu = item.GetCurrentParent() as ContextMenuStrip;
            PictureBox picture = menu.SourceControl as PictureBox;
            TableLayoutPanel table = splitContainer1.Panel1.Controls["table_layout_panel1"] as TableLayoutPanel;
            TableLayoutPanelCellPosition pos = table.GetPositionFromControl(picture);
            int x = pos.Row, y = pos.Column;
            int img_num = Data.Design_Field[x, y];
            Data.Field[x + 1, y + 1] = 0;
            switch (img_num)
            {
                case 11:
                case 12:
                    Data.Design_Field[x, y] = 10;
                    break;
                case 21:
                case 22:
                    Data.Design_Field[x, y] = 20;
                    break;
                case 31:
                case 32:
                    Data.Design_Field[x, y] = 30;
                    break;
                case 41:
                case 42:
                    Data.Design_Field[x, y] = 40;
                    break;
                case 51:
                case 52:
                    Data.Design_Field[x, y] = 50;
                    break;
                case 61:
                case 62:
                    Data.Design_Field[x, y] = 60;
                    break;
                case 71:
                case 72:
                    Data.Design_Field[x, y] = 70;
                    break;
                case 81:
                case 82:
                    Data.Design_Field[x, y] = 80;
                    break;
                case 91:
                case 92:
                    Data.Design_Field[x, y] = 90;
                    break;
            }
            Draw_One(x, y);
            picture.ContextMenuStrip.Items["delItem"].Enabled = false;
        }


        private void Stone_hover(object sender, EventArgs e)
        {
            PictureBox picture = sender as PictureBox;
            TableLayoutPanel table = splitContainer1.Panel1.Controls["table_layout_panel1"] as TableLayoutPanel;
            TableLayoutPanelCellPosition pos = table.GetPositionFromControl(sender as Control);
            int x = pos.Row, y = pos.Column;
            int img_num = Data.Design_Field[x, y];
            switch (img_num)
            {
                case 10:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 13;
                    else Data.Design_Field[x, y] = 14;
                    break;
                case 20:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 23;
                    else Data.Design_Field[x, y] = 24;
                    break;
                case 30:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 33;
                    else Data.Design_Field[x, y] = 34;
                    break;
                case 40:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 43;
                    else Data.Design_Field[x, y] = 44;
                    break;
                case 50:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 53;
                    else Data.Design_Field[x, y] = 54;
                    break;
                case 60:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 63;
                    else Data.Design_Field[x, y] = 64;
                    break;
                case 70:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 73;
                    else Data.Design_Field[x, y] = 74;
                    break;
                case 80:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 83;
                    else Data.Design_Field[x, y] = 84;
                    break;
                case 90:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 93;
                    else Data.Design_Field[x, y] = 94;
                    break;
            }
            Draw_One(x, y);
        }
        private void Stone_unhover(object sender, EventArgs e)
        {
            PictureBox picture = sender as PictureBox;
            TableLayoutPanel table = splitContainer1.Panel1.Controls["table_layout_panel1"] as TableLayoutPanel;
            TableLayoutPanelCellPosition pos = table.GetPositionFromControl(sender as Control);
            int x = pos.Row, y = pos.Column;
            int img_num = Data.Design_Field[x, y];
            switch (img_num)
            {
                case 13:
                case 14:
                    Data.Design_Field[x, y] = 10;
                    break;
                case 23:
                case 24:
                    Data.Design_Field[x, y] = 20;
                    break;
                case 33:
                case 34:
                    Data.Design_Field[x, y] = 30;
                    break;
                case 43:
                case 44:
                    Data.Design_Field[x, y] = 40;
                    break;
                case 53:
                case 54:
                    Data.Design_Field[x, y] = 50;
                    break;
                case 63:
                case 64:
                    Data.Design_Field[x, y] = 60;
                    break;
                case 73:
                case 74:
                    Data.Design_Field[x, y] = 70;
                    break;
                case 83:
                case 84:
                    Data.Design_Field[x, y] = 80;
                    break;
                case 93:
                case 94:
                    Data.Design_Field[x, y] = 90;
                    break;
            }
            Draw_One(x, y);
        }
        public void DeleteGroup()
        {
            for (int i = 1; i < Data.Height + 2; i++)
            {
                for (int j = 1; j < Data.Width + 2; j++)
                {
                    if (Data.Field[i, j] == 6)
                    {
                        Data.Field[i, j] = 0;
                    }
                    if (Data.Field[i, j] == 3 || Data.Field[i, j] == 4)
                    {
                        Data.Field[i, j] = 0;
                        Data.Current_move += 1;
                        TableLayoutPanel table = splitContainer1.Panel1.Controls["table_layout_panel1"] as TableLayoutPanel;
                        PictureBox picture = table.GetControlFromPosition(j - 1, i - 1) as PictureBox;
                        switch (Data.Design_Field[i - 1, j - 1] / 10)
                        {
                            case 1:
                                picture.Image = Properties.Resources.sprite;
                                Data.Design_Field[i - 1, j - 1] = 10;
                                break;
                            case 2:
                                picture.Image = Properties.Resources.top_edge;
                                Data.Design_Field[i - 1, j - 1] = 20;
                                break;
                            case 3:
                                picture.Image = Properties.Resources.down_edge;
                                Data.Design_Field[i - 1, j - 1] = 30;
                                break;
                            case 4:
                                picture.Image = Properties.Resources.right_edge;
                                Data.Design_Field[i - 1, j - 1] = 40;
                                break;
                            case 5:
                                picture.Image = Properties.Resources.left_edge;
                                Data.Design_Field[i - 1, j - 1] = 50;
                                break;
                            case 6:
                                picture.Image = Properties.Resources.right_top_corner;
                                Data.Design_Field[i - 1, j - 1] = 60;
                                break;
                            case 7:
                                picture.Image = Properties.Resources.left_top_corner;
                                Data.Design_Field[i - 1, j - 1] = 70;
                                break;
                            case 8:
                                picture.Image = Properties.Resources.right_down_corner;
                                Data.Design_Field[i - 1, j - 1] = 80;
                                break;
                            case 9:
                                picture.Image = Properties.Resources.left_down_corner;
                                Data.Design_Field[i - 1, j - 1] = 90;
                                break;
                        }
                    }
                }
            }
        }

        public int CountDame(int i, int j, char clr)
        {
            if (Data.Field[i, j] == 0)
            {
                Data.Field[i, j] = 6;
                return 1;
            }
            if (clr == 'b')
            {
                if (Data.Field[i, j] == 1)
                {
                    Data.Field[i, j] = 3;
                    return CountDame(i + 1, j, clr) + CountDame(i - 1, j, clr) + CountDame(i, j + 1, clr) + CountDame(i, j - 1, clr);
                }
            }
            else
            {
                if (Data.Field[i, j] == 2)
                {
                    Data.Field[i, j] = 4;
                    return CountDame(i + 1, j, clr) + CountDame(i - 1, j, clr) + CountDame(i, j + 1, clr) + CountDame(i, j - 1, clr);
                }
            }
            return 0;
        }
        public void ReturnDame()
        {
            for (int i = 0; i < Data.Height + 2; i++)
            {
                for (int j = 0; j < Data.Width + 2; j++)
                {
                    if (Data.Field[i, j] == 6) Data.Field[i, j] = 0;
                    if (Data.Field[i, j] == 3) Data.Field[i, j] = 1;
                    if (Data.Field[i, j] == 4) Data.Field[i, j] = 2;
                }
            }
        }
        public bool DeleteIf(int i, int j)
        {
            int dame = CountDame(i + 1, j, ReturnColor(i + 1, j));
            bool flag = false;
            if (dame == 0 && (Data.Color != ReturnColor(i + 1, j) && ReturnColor(i + 1, j) != 'n'))
            {
                DeleteGroup();
                flag = true;
            }
            ReturnDame();
            dame = CountDame(i, j + 1, ReturnColor(i, j + 1));
            if (dame == 0 && (Data.Color != ReturnColor(i, j + 1) && ReturnColor(i, j + 1) != 'n'))
            {
                DeleteGroup();
                flag = true;
            }
            ReturnDame();
            dame = CountDame(i - 1, j, ReturnColor(i - 1, j));
            if (dame == 0 && (Data.Color != ReturnColor(i - 1, j) && ReturnColor(i - 1, j) != 'n'))
            {
                DeleteGroup();
                flag = true;
            }
            ReturnDame();
            dame = CountDame(i, j - 1, ReturnColor(i, j - 1));
            {
                if (dame == 0 && (Data.Color != ReturnColor(i, j - 1) && ReturnColor(i, j - 1) != 'n'))
                {
                    DeleteGroup();
                    flag = true;
                }
            }
            ReturnDame();
            return flag;
        }
        public int CountDameWithReturning(int i, int j)
        {
            int num = CountDame(i, j, ReturnColor(i, j));
            ReturnDame();
            return num;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox picture = sender as PictureBox;
            picture.ContextMenuStrip.Items["delItem"].Enabled = true;
            TableLayoutPanel table = splitContainer1.Panel1.Controls["table_layout_panel1"] as TableLayoutPanel;
            TableLayoutPanelCellPosition pos = table.GetPositionFromControl(sender as Control);
            int x = pos.Row, y = pos.Column;
            int img_num = Data.Design_Field[x, y];
            int[,] start = CreateAndFill(Data.Width + 2, Data.Height + 2, Data.Field);
            bool flag1;
            if (Data.Field[x + 1, y + 1] != 1 && Data.Field[x + 1, y + 1] != 2)
            {
                if (Data.Color == 'b') Data.Field[x + 1, y + 1] = 1;
                else Data.Field[x + 1, y + 1] = 2;
            }
            else return;
            if (free_mode.Enabled)
            {
                flag1 = DeleteIf(x + 1, y + 1);
                if (flag1 == false && CountDameWithReturning(x + 1, y + 1) == 0)
                {
                    Data.Field = start;
                    return;
                }
            }
            Data.Current_move += 1;
            if (Data.Color == 'b') Data.LastMoveB = CreateAndFill(Data.Width + 2, Data.Height + 2, Data.Field);
            else Data.LastMoveW = CreateAndFill(Data.Width + 2, Data.Height + 2, Data.Field);

            switch (img_num)
            {
                case 13:
                case 14:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 11;
                    else Data.Design_Field[x, y] = 12;
                    break;
                case 23:
                case 24:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 21;
                    else Data.Design_Field[x, y] = 22;
                    break;
                case 33:
                case 34:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 31;
                    else Data.Design_Field[x, y] = 32;
                    break;
                case 43:
                case 44:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 41;
                    else Data.Design_Field[x, y] = 42;
                    break;
                case 53:
                case 54:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 51;
                    else Data.Design_Field[x, y] = 52;
                    break;
                case 63:
                case 64:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 61;
                    else Data.Design_Field[x, y] = 62;
                    break;
                case 73:
                case 74:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 71;
                    else Data.Design_Field[x, y] = 72;
                    break;
                case 83:
                case 84:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 81;
                    else Data.Design_Field[x, y] = 82;
                    break;
                case 93:
                case 94:
                    if (Data.Color == 'b') Data.Design_Field[x, y] = 91;
                    else Data.Design_Field[x, y] = 92;
                    break;
            }
            Draw_One(x, y);

            if (treeView1.Nodes.Count != 0 && treeView1.SelectedNode != null)
            {
                string s = Convert.ToString(Data.Color).ToUpper();
                TreeNode new_node = new TreeNode(s + "[" + Data.Alphabet[y] + ";" + Data.Alphabet[x] + "]");
                treeView1.SelectedNode.Nodes.Add(new_node);
                treeView1.SelectedNode.Expand();
                treeView1.SelectedNode = new_node;
                Node_Data nd = new Node_Data();
                nd.Field = Data.Field.Clone() as int[,];
                nd.Design_Field = Data.Design_Field.Clone() as int[,];
                nd.Comment = "";
                treeView1.SelectedNode.Tag = nd;
            }
            if (play_mode.Enabled == false) ChangeColor(sender, e);
        }
        public bool IsEqualPosition(int col, int row, int[,] t1, int[,] t2)
        {
            for (int i = 1; i < row; i++)
            {
                for (int j = 1; j < col; j++)
                {
                    if (t1[i, j] != t2[i, j])
                        return false;
                }
            }
            return true;
        }


        private void ChangeColor(object sender, EventArgs e)
        {
            if (Data.Color == 'b')
                Data.Color = 'w';
            else if (Data.Color == 'w')
                Data.Color = 'b';
        }
        public char ReturnColor(int i, int j)
        {
            switch (Data.Field[i, j])
            {
                case 1:
                case 3:
                    return 'b';
                case 2:
                case 4:
                    return 'w';
            }
            return 'n';
        }

        public void Draw_One(int i, int j)
        {
            // i - номер строки, j - номер столбца
            TableLayoutPanel table = splitContainer1.Panel1.Controls["table_layout_panel1"] as TableLayoutPanel;
            PictureBox picture = table.GetControlFromPosition(j, i) as PictureBox;
            switch (Data.Design_Field[i, j])
            {
                case 10:
                    picture.Image = Properties.Resources.sprite;
                    break;
                case 11:
                    picture.Image = Properties.Resources.sprite_b;
                    break;
                case 12:
                    picture.Image = Properties.Resources.sprite_w;
                    break;
                case 13:
                    picture.Image = Properties.Resources.sprite_hover_b;
                    break;
                case 14:
                    picture.Image = Properties.Resources.sprite_hover_w;
                    break;
                case 20:
                    picture.Image = Properties.Resources.top_edge;
                    break;
                case 21:
                    picture.Image = Properties.Resources.top_edge_b;
                    break;
                case 22:
                    picture.Image = Properties.Resources.top_edge_w;
                    break;
                case 23:
                    picture.Image = Properties.Resources.top_edge_hover_b;
                    break;
                case 24:
                    picture.Image = Properties.Resources.top_edge_hover_w;
                    break;
                case 30:
                    picture.Image = Properties.Resources.down_edge;
                    break;
                case 31:
                    picture.Image = Properties.Resources.down_edge_b;
                    break;
                case 32:
                    picture.Image = Properties.Resources.down_edge_w;
                    break;
                case 33:
                    picture.Image = Properties.Resources.down_edge_hover_b;
                    break;
                case 34:
                    picture.Image = Properties.Resources.down_edge_hover_w;
                    break;
                case 40:
                    picture.Image = Properties.Resources.right_edge;
                    break;
                case 41:
                    picture.Image = Properties.Resources.right_edge_b;
                    break;
                case 42:
                    picture.Image = Properties.Resources.right_edge_w;
                    break;
                case 43:
                    picture.Image = Properties.Resources.right_edge_hover_b;
                    break;
                case 44:
                    picture.Image = Properties.Resources.right_edge_hover_w;
                    break;
                case 50:
                    picture.Image = Properties.Resources.left_edge;
                    break;
                case 51:
                    picture.Image = Properties.Resources.left_edge_b;
                    break;
                case 52:
                    picture.Image = Properties.Resources.left_edge_w;
                    break;
                case 53:
                    picture.Image = Properties.Resources.left_edge_hover_b;
                    break;
                case 54:
                    picture.Image = Properties.Resources.left_edge_hover_w;
                    break;
                case 60:
                    picture.Image = Properties.Resources.right_top_corner;
                    break;
                case 61:
                    picture.Image = Properties.Resources.right_top_corner_b;
                    break;
                case 62:
                    picture.Image = Properties.Resources.right_top_corner_w;
                    break;
                case 63:
                    picture.Image = Properties.Resources.right_top_corner_hover_b;
                    break;
                case 64:
                    picture.Image = Properties.Resources.right_top_corner_hover_w;
                    break;
                case 70:
                    picture.Image = Properties.Resources.left_top_corner;
                    break;
                case 71:
                    picture.Image = Properties.Resources.left_top_corner_b;
                    break;
                case 72:
                    picture.Image = Properties.Resources.left_top_corner_w;
                    break;
                case 73:
                    picture.Image = Properties.Resources.left_top_corner_hover_b;
                    break;
                case 74:
                    picture.Image = Properties.Resources.left_top_corner_hover_w;
                    break;
                case 80:
                    picture.Image = Properties.Resources.right_down_corner;
                    break;
                case 81:
                    picture.Image = Properties.Resources.right_down_corner_b;
                    break;
                case 82:
                    picture.Image = Properties.Resources.right_down_corner_w;
                    break;
                case 83:
                    picture.Image = Properties.Resources.right_down_corner_hover_b;
                    break;
                case 84:
                    picture.Image = Properties.Resources.right_down_corner_hover_w;
                    break;
                case 90:
                    picture.Image = Properties.Resources.left_down_corner;
                    break;
                case 91:
                    picture.Image = Properties.Resources.left_down_corner_b;
                    break;
                case 92:
                    picture.Image = Properties.Resources.left_down_corner_w;
                    break;
                case 93:
                    picture.Image = Properties.Resources.left_down_corner_hover_b;
                    break;
                case 94:
                    picture.Image = Properties.Resources.left_down_corner_hover_w;
                    break;
            }
        }
        public void Draw_All()
        {
            for (int i = 0; i < Data.Height; i++)
            {
                for (int j = 0; j < Data.Width; j++)
                {
                    Draw_One(i, j);
                }
            }
        }

        public static class Data
        {
            public static char Color { get; set; }
            /* Первая цифра - тип пересечения: 1 - центр, 2 - верхний край, 3 - нижний край, 4 - правый
             * край, 5 - левый край, 6 - правый верхний угол, 7 - левый верхний угол, 8 - правый нижний
             * угол, 9 левый нижний угол
             * Вторая цифра - тип камня: 0 - пусто, 1 -черный, 2 - белый, 3 - черный полупрозрачный,
             * 4 - белый полупрозрачный
             */
            public static int[,] Design_Field { get; set; }
            /* 0 - пусто, 1 - черный камень, 2 - белый камень, 3 -черный отмеченный, 4 - белый отмеченный,
             * 5 - дыхание, 6 - отмеченное дыхание, 7 - край
             */
            public static int[,] Field { get; set; }
            // Две копии для проверки правила ко.
            public static int[,] LastMoveB { get; set; }
            public static int[,] LastMoveW { get; set; }
            public static int Width { get; set; }
            public static int Height { get; set; }
            public static string Alphabet { get; set; }
            // Два символа - сначала столбец, потом строка
            public static string[] Moves { get; set; }
            public static int Current_move { get; set; }
            public static string Str_for_save { get; set;}
        }

        private void free_mode_Click(object sender, EventArgs e)
        {
            free_mode.Enabled = false;
            play_mode.Enabled = true;
        }

        private void play_mode_Click(object sender, EventArgs e)
        {
            play_mode.Enabled = false;
            free_mode.Enabled = true;
        }

        struct Node_Data
        {
            public int[,] Field;
            public int[,] Design_Field;
            public string Comment;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node.Tag is null) return;
            Node_Data? st = node.Tag as Node_Data?;
            Data.Field = st.Value.Field.Clone() as int[,];
            Data.Design_Field = st.Value.Design_Field.Clone() as int[,];
            Draw_All();
        }

        public void ChangeComment(string text)
        {
            Node_Data nd = new Node_Data();
            nd.Comment = text;
            TreeNode node = treeView1.SelectedNode;
            Node_Data? st = node.Tag as Node_Data?;
            nd.Field = st.Value.Field;
            nd.Design_Field = st.Value.Design_Field;
            treeView1.SelectedNode.Tag = nd;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode is null) return;
            TreeNode node = treeView1.SelectedNode;
            Node_Data? st = node.Tag as Node_Data?;
            Form2 frm = new Form2(this);
            frm.Controls["textBox1"].Text = st.Value.Comment;
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeView1.SelectedNode = treeView1.TopNode;
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) return;
            if (treeView1.Nodes.Count == 0) return;
            string filename = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(filename, CreateString(filename));
            MessageBox.Show("Файл сохранен");
        }
     
        string[] BlackAndWhite()
        {
            string first = "";
            string second = "";
            Node_Data? nd = treeView1.TopNode.Tag as Node_Data?;
            for (int i = 0; i < Data.Height; i++)
            {
                for (int j = 0; j < Data.Width; j++)
                {
                    if (nd.Value.Design_Field[i, j] % 10 == 1) first += "[" + Data.Alphabet[j] + Data.Alphabet[i] + "]";
                    if (nd.Value.Design_Field[i, j] % 10 == 2) second += "[" + Data.Alphabet[j] + Data.Alphabet[i] + "]";
                }
            }
            string[] arr = { first, second };
            return arr;
        }

        void RecursiveNode(TreeNode node)
        {
            if (node.Nodes.Count == 0)
            {
                Data.Str_for_save += ";" + node.Text[0] + "[" + node.Text[2] + node.Text[4] + "]";
                Node_Data? nd = node.Tag as Node_Data?;
                if (nd.Value.Comment != "")
                {
                    Data.Str_for_save += "C[" + nd.Value.Comment + "]";  
                }
                Data.Str_for_save += ")";
                return;
            }
            else if (node.Nodes.Count == 1)
            {
                Data.Str_for_save += ";" + node.Text[0] + "[" + node.Text[2] + node.Text[4] + "]";
                Node_Data? nd = node.Tag as Node_Data?;
                if (nd.Value.Comment != "")
                {
                    Data.Str_for_save += "C[" + nd.Value.Comment + "]";
                }
                RecursiveNode(node.FirstNode);
            }
            else
            {
                Data.Str_for_save += ";" + node.Text[0] + "[" + node.Text[2] + node.Text[4] + "]";
                Node_Data? nd = node.Tag as Node_Data?;
                if (nd.Value.Comment != "")
                {
                    Data.Str_for_save += "C[" + nd.Value.Comment + "]";
                }
                foreach (TreeNode n in node.Nodes)
                {
                    Data.Str_for_save += "(";
                    RecursiveNode(n);
                }
                Data.Str_for_save += ")";
                return;
            }

        }
        string CreateString(string fname)
        {
            Data.Str_for_save = "";
            Data.Str_for_save += "(;GM[1]FF[4]SZ[" + Data.Width.ToString() + "]HA[0]KM[0]GN[" + fname + "]";
            string[] start_stones = BlackAndWhite();
            if (start_stones[0] != "") Data.Str_for_save += "AB" + start_stones[0];
            if (start_stones[1] != "") Data.Str_for_save += "AW" + start_stones[1];
            Node_Data? nd = treeView1.TopNode.Tag as Node_Data?;
            Data.Str_for_save += "C[" + nd.Value.Comment + "]";
            foreach (TreeNode n in treeView1.TopNode.Nodes)
            {
                Data.Str_for_save += "(";
                RecursiveNode(n);
            }
            Data.Str_for_save += ")";
            return Data.Str_for_save;
        }
    }
}
