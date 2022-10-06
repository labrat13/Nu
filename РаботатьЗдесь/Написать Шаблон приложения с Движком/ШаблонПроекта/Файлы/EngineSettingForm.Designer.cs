namespace PMEngine
{
    partial class EngineSettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EngineSettingForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_Main = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox_ReadOnly = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Version = new System.Windows.Forms.TextBox();
            this.textBox_Title = new System.Windows.Forms.TextBox();
            this.textBox_Descr = new System.Windows.Forms.TextBox();
            this.textBox_Creator = new System.Windows.Forms.TextBox();
            this.textBox_Directory = new System.Windows.Forms.TextBox();
            this.textBox_Class = new System.Windows.Forms.TextBox();
            this.button_Path = new System.Windows.Forms.Button();
            this.tabPage_Other = new System.Windows.Forms.TabPage();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage_Main.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.Controls.Add(this.button_OK, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.button_Cancel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(342, 256);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(175, 229);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 0;
            this.button_OK.Text = "ОК";
            this.toolTip1.SetToolTip(this.button_OK, "Сохранить изменения");
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(260, 229);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "Отмена";
            this.toolTip1.SetToolTip(this.button_Cancel, "Не сохранять изменения");
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // tabControl1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabControl1, 3);
            this.tabControl1.Controls.Add(this.tabPage_Main);
            this.tabControl1.Controls.Add(this.tabPage_Other);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(336, 220);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage_Main
            // 
            this.tabPage_Main.Controls.Add(this.tableLayoutPanel2);
            this.tabPage_Main.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Main.Name = "tabPage_Main";
            this.tabPage_Main.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Main.Size = new System.Drawing.Size(328, 194);
            this.tabPage_Main.TabIndex = 0;
            this.tabPage_Main.Text = "Основные";
            this.toolTip1.SetToolTip(this.tabPage_Main, "Вкладка Основные свойства");
            this.tabPage_Main.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.checkBox_ReadOnly, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Version, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Title, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Descr, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Creator, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Directory, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Class, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.button_Path, 2, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(322, 188);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.MediumBlue;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Название: (*)";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Описание:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Создатель:";
            // 
            // checkBox_ReadOnly
            // 
            this.checkBox_ReadOnly.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBox_ReadOnly.AutoSize = true;
            this.checkBox_ReadOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tableLayoutPanel2.SetColumnSpan(this.checkBox_ReadOnly, 2);
            this.checkBox_ReadOnly.Location = new System.Drawing.Point(3, 82);
            this.checkBox_ReadOnly.Name = "checkBox_ReadOnly";
            this.checkBox_ReadOnly.Size = new System.Drawing.Size(103, 17);
            this.checkBox_ReadOnly.TabIndex = 15;
            this.checkBox_ReadOnly.Text = "Только чтение:";
            this.toolTip1.SetToolTip(this.checkBox_ReadOnly, "Установите флажок, если изменения в проекте недопустимы");
            this.checkBox_ReadOnly.UseVisualStyleBackColor = true;
            this.checkBox_ReadOnly.CheckedChanged += new System.EventHandler(this.Control_TextChanged);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.MediumBlue;
            this.label6.Location = new System.Drawing.Point(3, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Каталог: (*)";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Класс движка:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Версия движка:";
            // 
            // textBox_Version
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.textBox_Version, 2);
            this.textBox_Version.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Version.Location = new System.Drawing.Point(98, 159);
            this.textBox_Version.Name = "textBox_Version";
            this.textBox_Version.ReadOnly = true;
            this.textBox_Version.Size = new System.Drawing.Size(221, 20);
            this.textBox_Version.TabIndex = 19;
            // 
            // textBox_Title
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.textBox_Title, 2);
            this.textBox_Title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Title.Location = new System.Drawing.Point(98, 3);
            this.textBox_Title.Name = "textBox_Title";
            this.textBox_Title.Size = new System.Drawing.Size(221, 20);
            this.textBox_Title.TabIndex = 20;
            this.toolTip1.SetToolTip(this.textBox_Title, "Введите название проекта");
            this.textBox_Title.TextChanged += new System.EventHandler(this.Control_TextChanged);
            // 
            // textBox_Descr
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.textBox_Descr, 2);
            this.textBox_Descr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Descr.Location = new System.Drawing.Point(98, 29);
            this.textBox_Descr.Name = "textBox_Descr";
            this.textBox_Descr.Size = new System.Drawing.Size(221, 20);
            this.textBox_Descr.TabIndex = 21;
            this.textBox_Descr.Text = " ";
            this.toolTip1.SetToolTip(this.textBox_Descr, "Введите краткое описание проекта");
            this.textBox_Descr.TextChanged += new System.EventHandler(this.Control_TextChanged);
            // 
            // textBox_Creator
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.textBox_Creator, 2);
            this.textBox_Creator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Creator.Location = new System.Drawing.Point(98, 55);
            this.textBox_Creator.Name = "textBox_Creator";
            this.textBox_Creator.Size = new System.Drawing.Size(221, 20);
            this.textBox_Creator.TabIndex = 22;
            this.toolTip1.SetToolTip(this.textBox_Creator, "Введите название создателя проекта");
            this.textBox_Creator.TextChanged += new System.EventHandler(this.Control_TextChanged);
            // 
            // textBox_Directory
            // 
            this.textBox_Directory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Directory.Location = new System.Drawing.Point(98, 107);
            this.textBox_Directory.Name = "textBox_Directory";
            this.textBox_Directory.ReadOnly = true;
            this.textBox_Directory.Size = new System.Drawing.Size(181, 20);
            this.textBox_Directory.TabIndex = 23;
            this.toolTip1.SetToolTip(this.textBox_Directory, "Путь к каталогу данных проекта");
            this.textBox_Directory.TextChanged += new System.EventHandler(this.Control_TextChanged);
            // 
            // textBox_Class
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.textBox_Class, 2);
            this.textBox_Class.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Class.Location = new System.Drawing.Point(98, 133);
            this.textBox_Class.Name = "textBox_Class";
            this.textBox_Class.ReadOnly = true;
            this.textBox_Class.Size = new System.Drawing.Size(221, 20);
            this.textBox_Class.TabIndex = 24;
            // 
            // button_Path
            // 
            this.button_Path.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button_Path.Location = new System.Drawing.Point(282, 105);
            this.button_Path.Margin = new System.Windows.Forms.Padding(0);
            this.button_Path.Name = "button_Path";
            this.button_Path.Size = new System.Drawing.Size(40, 23);
            this.button_Path.TabIndex = 25;
            this.button_Path.Text = "...";
            this.toolTip1.SetToolTip(this.button_Path, "Кликните чтобы изменить путь к каталогу");
            this.button_Path.UseVisualStyleBackColor = true;
            this.button_Path.Click += new System.EventHandler(this.button_Path_Click);
            // 
            // tabPage_Other
            // 
            this.tabPage_Other.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Other.Name = "tabPage_Other";
            this.tabPage_Other.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Other.Size = new System.Drawing.Size(328, 194);
            this.tabPage_Other.TabIndex = 1;
            this.tabPage_Other.Text = "Другие";
            this.toolTip1.SetToolTip(this.tabPage_Other, "Вкладка Прочие свойства");
            this.tabPage_Other.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 234);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "* - Обязательное поле";
            // 
            // EngineSettingForm
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(342, 256);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 290);
            this.Name = "EngineSettingForm";
            this.Text = "EngineSettingForm";
            this.Load += new System.EventHandler(this.EngineSettingForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EngineSettingForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage_Main.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_Main;
        private System.Windows.Forms.TabPage tabPage_Other;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_ReadOnly;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Version;
        private System.Windows.Forms.TextBox textBox_Title;
        private System.Windows.Forms.TextBox textBox_Descr;
        private System.Windows.Forms.TextBox textBox_Creator;
        private System.Windows.Forms.TextBox textBox_Directory;
        private System.Windows.Forms.TextBox textBox_Class;
        private System.Windows.Forms.Button button_Path;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label7;
    }
}