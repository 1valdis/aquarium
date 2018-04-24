using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Aquarium
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HereWeGo = new Aquarium(tableLayoutPanel1.RowCount, tableLayoutPanel1.ColumnCount);
            HereWeGo.Territory[1, 1] = new PredatorFish(HereWeGo, 1, 1, true, 100, 30);
            HereWeGo.Territory[2, 10] = new PredatorFish(HereWeGo, 2, 10, false, 100, 100);
            HereWeGo.Territory[2, 3] = new PredatorFish(HereWeGo, 2, 3, true, 100, 30);
            HereWeGo.Territory[2, 13] = new PredatorFish(HereWeGo, 2, 13, false, 130, 100, 10);
            HereWeGo.Territory[5, 3] = new HerbivoreFish(HereWeGo, 5, 3, true, 100, 100);
            HereWeGo.Territory[6, 12] = new HerbivoreFish(HereWeGo, 6, 12, false, 100, 100, pregnancyTime: 7);
            HereWeGo.Territory[7, 6] = new HerbivoreFish(HereWeGo, 7, 6, true, 150, 0);
            HereWeGo.Territory[8, 11] = new HerbivoreFish(HereWeGo, 8, 11, false, 150, 20, pregnancyTime: 9);
            HereWeGo.Territory[9, 4] = new Seaweed(HereWeGo, 9, 4);
            HereWeGo.Territory[9, 5] = new Seaweed(HereWeGo, 9, 5);
            HereWeGo.Territory[9, 6] = new Seaweed(HereWeGo, 9, 6);
            HereWeGo.Territory[9, 8] = new Seaweed(HereWeGo, 9, 8);
            HereWeGo.Territory[9, 9] = new Seaweed(HereWeGo, 9, 9);
            HereWeGo.Territory[8, 9] = new Seaweed(HereWeGo, 8, 9);
            HereWeGo.Territory[9, 1] = new Rock(HereWeGo, 9, 1);
            HereWeGo.Territory[9, 2] = new Rock(HereWeGo, 9, 2);
            HereWeGo.Territory[9, 3] = new Rock(HereWeGo, 9, 3);
            DrawAquarium(HereWeGo, tableLayoutPanel1);
            GenderField.SelectedItem = "Male";
            comboBox1.SelectedItem = "Empty";
        }

        protected int selectedX, selectedY;
        Aquarium HereWeGo;

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            selectedX = tableLayoutPanel1.GetCellPosition(pb).Row;
            selectedY = tableLayoutPanel1.GetCellPosition(pb).Column;
            AquariumObject SelectedObject = HereWeGo.Territory[selectedX, selectedY];
            if (SelectedObject == null)
                comboBox1.SelectedItem = "Empty";
            if (SelectedObject is Rock)
                comboBox1.SelectedItem = "Rock";
            if (SelectedObject is Seaweed)
                comboBox1.SelectedItem = "Seaweed";
            if (SelectedObject is HerbivoreFish)
                comboBox1.SelectedItem = "Herbivore";
            if (SelectedObject is PredatorFish)
                comboBox1.SelectedItem = "Predator";
            ActivateAppropriateFields(comboBox1.SelectedItem.ToString());
        }

        private void InactivateAllControls()
        {
            AgeField.Enabled = false;
            FoodField.Enabled = false;
            SatietyField.Enabled = false;
            GenderField.Enabled = false;
            PregnancyField.Enabled = false;
        }
        private void ActivateAppropriateFields(string className)
        {
            InactivateAllControls();
            switch (className)
            {
                case "Seaweed":
                    FoodField.Enabled = true;
                    break;
                case "Herbivore":
                    AgeField.Enabled = true;
                    FoodField.Enabled = true;
                    SatietyField.Enabled = true;
                    GenderField.Enabled = true;
                    PregnancyField.Enabled = true;
                    break;
                case "Predator":
                    AgeField.Enabled = true;
                    SatietyField.Enabled = true;
                    GenderField.Enabled = true;
                    PregnancyField.Enabled = true;
                    break;
                default: break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AquariumObject ToInsert;
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Rock":
                    ToInsert = new Rock(HereWeGo, selectedX, selectedY);
                    break;
                case "Seaweed":
                    ToInsert = new Seaweed(HereWeGo, selectedX, selectedY, (int)FoodField.Value);
                    break;
                case "Herbivore":
                    ToInsert = new HerbivoreFish(HereWeGo, selectedX, selectedY, GenderField.SelectedItem.ToString() == "Male" ? true : false, (int)AgeField.Value, (int)SatietyField.Value, (int)FoodField.Value, (int)PregnancyField.Value);
                    break;
                case "Predator":
                    ToInsert = new PredatorFish(HereWeGo, selectedX, selectedY, GenderField.SelectedItem.ToString() == "Male" ? true : false, (int)AgeField.Value, (int)SatietyField.Value, (int)PregnancyField.Value);
                    break;
                default:
                    ToInsert = null;
                    break;
            }
            HereWeGo.Territory[selectedX, selectedY] = ToInsert;
            DrawAquarium(HereWeGo, tableLayoutPanel1);
        }

        private void NextIterationButton_Click(object sender, EventArgs e)
        {
            HereWeGo.NextIteration();
            DrawAquarium(HereWeGo, tableLayoutPanel1);
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ActivateAppropriateFields(comboBox1.SelectedItem.ToString());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            HereWeGo.NextIteration();
            DrawAquarium(HereWeGo, tableLayoutPanel1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void DrawAquarium(Aquarium aquarium, TableLayoutPanel panel)
        {
            AquariumObject[,] Territory = aquarium.Territory;
            for (int i = 0; i < Territory.GetLength(0); i++)
            {
                for (int j = 0; j < Territory.GetLength(1); j++)
                {
                    PictureBox pic=null;
                    try
                    {
                        pic = panel.Controls.Find($"Pic{j}-{i}", false)[0] as PictureBox;
                    }
                    catch
                    {
                        pic = new PictureBox();
                        pic.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        pic.Name = $"Pic{j}-{i}";
                        pic.Click += PictureBox_Click;
                        pic.SizeMode = PictureBoxSizeMode.Zoom;
                        panel.Controls.Add(pic, j, i);
                    }
                    finally
                    {
                        pic.Image = Territory[i, j] == null ? Properties.Resources.water : Territory[i, j].Image;
                    }
                }
            }
        }
    }
}
