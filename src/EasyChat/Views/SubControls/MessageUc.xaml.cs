using System.Windows.Controls;

namespace EasyChat.Views.SubControls
{
    /// <summary>
    /// MessageUc.xaml 的交互逻辑
    /// </summary>
    public partial class MessageUc : UserControl
    {
        public MessageUc()
        {
            InitializeComponent();
            MessageListBox.Items.CurrentChanged += (s, e) =>
            {
                if (MessageListBox.Items.Count > 0)
                {
                    MessageListBox.ScrollIntoView(MessageListBox.Items[MessageListBox.Items.Count - 1]);
                }
            };
        }
    }

}
