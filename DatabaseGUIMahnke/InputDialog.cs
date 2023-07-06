using System.Windows.Forms;

public class InputDialog : Form
{
    private TextBox textBox;
    private Button okButton;

    public InputDialog()
    {
        InitializeComponent();
    }

    public string ConnectionString
    {
        get { return textBox.Text; }
    }

    private void InitializeComponent()
    {
        textBox = new TextBox();
        okButton = new Button();

        // Set form properties
        this.Text = "Enter Connection String";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;

        // Set textBox properties
        textBox.Dock = DockStyle.Top;

        // Set okButton properties
        okButton.Text = "OK";
        okButton.DialogResult = DialogResult.OK;
        okButton.Dock = DockStyle.Top;

        // Add controls to the form
        this.Controls.Add(textBox);
        this.Controls.Add(okButton);

        // Set form size and layout
        this.ClientSize = new System.Drawing.Size(400, 100);
    }
}
