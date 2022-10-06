using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PMEngine
{
    /// <summary>
    /// Показывает форму редактирования объекта EngineSettings
    /// </summary>
    public partial class EngineSettingForm : Form
    {
        /* Примененный паттерн:
         * Когда форма показывается пользователю, в ее контролы вносятся данные из объекта. В обработчике Form_Load.
         * Кнопка ОК записывает данные из контролов формы в объект и закрывает форму.
         * Кнопка Отмена закрывает форму без сохранения данных.
         * Изначально кнопка ОК выключена.
         * Когда пользователь изменяет текст какого-либо из контролов, срабатывает подключенный обработчик Control_TextChanged.
         * Он включает кнопку ОК.
         * Пользователь, нажав кнопку ОК, записывает данные из контролов в объект и закрывает форму.
         * 
         * Объект передается в форму через проперти Item.
         * 
         * Форма может быть открыта только для просмотра данных. 
         * Флаг задается через проперти ReadOnly.
         * Если флаг ReadOnly установлен, все контролы TextBox переводятся в режим ReadOnly, 
         *  а контролы других типов выключаются (.Enabled = false); дополнительно, обработчик Control_TextChanged не включает кнопку ОК.
         * Поэтому в этом режиме нельзя изменить данные объекта Item.
         * 
         * Для использования формы создана функция - обертка
         * public static DialogResult ShowForm(IWin32Window owner, EngineSettings item, bool readOnly)
         * Она возвращает нажатую кнопку как DialogResult. Значение ОК показывает, что объект Item был изменен.
         */


        /// <summary>
        /// Флаг Только чтение формы
        /// </summary>
        private bool m_ReadOnly;

        /// <summary>
        /// Объект настроек для редактирования
        /// </summary>
        private EngineSettings m_Item;

        /// <summary>
        /// Default constructor
        /// </summary>
        public EngineSettingForm()
        {
            InitializeComponent();
        }

        #region Properties
        /// <summary>
        /// Флаг Только чтение формы
        /// </summary>
        public bool ReadOnly
        {
            get { return m_ReadOnly; }
            set { m_ReadOnly = value; }
        }
        /// <summary>
        /// Объект настроек для редактирования
        /// </summary>
        public EngineSettings Item
        {
            get { return m_Item; }
            set { m_Item = value; }
        }
        #endregion

        #region Form Handlers
        /// <summary>
        /// NT-Показать форму с данными объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngineSettingForm_Load(object sender, EventArgs e)
        {
            //вписать данные в контролы формы
            //вкладка TabPageMain:
            this.textBox_Class.Text = m_Item.EngineClass;
            this.textBox_Creator.Text = m_Item.Creator;
            this.textBox_Descr.Text = m_Item.Description;
            this.textBox_Directory.Text = m_Item.StoragePath;
            this.textBox_Title.Text = m_Item.Title;
            this.textBox_Version.Text = m_Item.EngineVersion;
            this.checkBox_ReadOnly.Checked = m_Item.ReadOnly;
            //TODO: добавить данные новых контролов формы тут

            //выключить контролы если включен режим только чтение
            setReadOnlyMode(this.m_ReadOnly);
            //выключить кнопку ОК после заполнения полей.
            //когда пользователь сделает изменения в данных, она разблокируется по событию Control_TextChanged()
            this.button_OK.Enabled = false;

            return;
        }
        /// <summary>
        /// NT-Обработчик события Форма закрывается
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngineSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //тут вроде нечего делать...
        }
        /// <summary>
        /// NT-Обработчик события Нажата кнопка Отмена или крестик в заголовке окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            //ничего не сохраняем в объект настроек
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        /// <summary>
        /// NT-Обработчик события Нажата кнопка ОК
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_OK_Click(object sender, EventArgs e)
        {
            //сохраняем данные из контролов формы в объект настроек
            //вкладка TabPageMain:
            m_Item.Creator = this.textBox_Creator.Text.Trim();
            m_Item.Description = this.textBox_Descr.Text.Trim();
            m_Item.StoragePath = this.textBox_Directory.Text.Trim();
            m_Item.Title = this.textBox_Title.Text.Trim();
            m_Item.ReadOnly = this.checkBox_ReadOnly.Checked;
            //m_Item.EngineClass   - не должны изменяться
            //m_Item.EngineVersion - не должны изменяться
            //TODO: добавить данные новых контролов формы тут

            //закрываем форму
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// NT-Событие изменения состояния контролов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_TextChanged(object sender, EventArgs e)
        {
            //было изменение в содержимом контролов
            //включить кнопку ОК для записи изменений в объект настроек.
            if (this.m_ReadOnly == false)
                this.button_OK.Enabled = true;

            return;
        }
        /// <summary>
        /// NT-Обработчик кнопки Выбрать каталог для проекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Path_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Выберите родительский каталог для нового проекта данных";
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.SelectedPath = this.m_Item.StoragePath;
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog(this) == DialogResult.OK)
                this.textBox_Directory.Text = fbd.SelectedPath;

            return;
        }

        #endregion

        /// <summary>
        /// NT-Показать объект настроек для редактирования и просмотра
        /// </summary>
        /// <param name="item">Объект настроек</param>
        /// <param name="readOnly">Только чтение</param>
        /// <returns>Возвращает значение DialogResult формы</returns>
        public static DialogResult ShowForm(IWin32Window owner, EngineSettings item, bool readOnly)
        {
            EngineSettingForm f = new EngineSettingForm();
            f.ReadOnly = readOnly;
            f.Item = item;
            return f.ShowDialog(owner);
        }

        /// <summary>
        /// NT- Установить режим только чтение для контролов формы
        /// </summary>
        /// <param name="readOnly"></param>
        private void setReadOnlyMode(bool readOnly)
        {
            //setup tabPageMain controls
            this.checkBox_ReadOnly.Enabled = !readOnly;
            this.button_OK.Enabled = !readOnly;
            this.button_Path.Enabled = !readOnly;
            this.textBox_Descr.ReadOnly = readOnly;
            this.textBox_Title.ReadOnly = readOnly;
            this.textBox_Creator.ReadOnly = readOnly;
            this.textBox_Class.ReadOnly = true;
            this.textBox_Directory.ReadOnly = true;
            this.textBox_Version.ReadOnly = true;
            //TODO: тут установить режим только чтение для новых контролов формы
            return;
        }


    }
}
