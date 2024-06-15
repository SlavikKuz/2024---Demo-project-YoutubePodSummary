namespace YoutubePodSmart.WinForms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        GetVideoButton = new Button();
        videoSourceTextBox = new TextBox();
        audioFormatRadioButton = new RadioButton();
        GetAudioButton = new Button();
        progressBar = new ProgressBar();
        statusLabel = new Label();
        TranscribeButton = new Button();
        transcriptTextBox = new TextBox();
        GetSummaryButton = new Button();
        progressTimer = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        //
        // Timer
        //
        progressTimer.Interval = 3000;
        progressTimer.Tick += ProgressTimer_Tick;
        // 
        // GetVideoButton
        // 
        GetVideoButton.Location = new Point(703, 24);
        GetVideoButton.Name = "GetVideoButton";
        GetVideoButton.Size = new Size(75, 23);
        GetVideoButton.TabIndex = 0;
        GetVideoButton.Text = "Get video";
        GetVideoButton.UseVisualStyleBackColor = true;
        GetVideoButton.Click += GetVideoButton_Click;
        // 
        // videoSourceTextBox
        // 
        videoSourceTextBox.Location = new Point(23, 24);
        videoSourceTextBox.Name = "videoSourceTextBox";
        videoSourceTextBox.PlaceholderText = "Insert link to youtube video";
        videoSourceTextBox.Size = new Size(651, 23);
        videoSourceTextBox.TabIndex = 1;
        // 
        // audioFormatRadioButton
        // 
        audioFormatRadioButton.AutoSize = true;
        audioFormatRadioButton.Checked = true;
        audioFormatRadioButton.Location = new Point(348, 69);
        audioFormatRadioButton.Name = "audioFormatRadioButton";
        audioFormatRadioButton.Size = new Size(49, 19);
        audioFormatRadioButton.TabIndex = 2;
        audioFormatRadioButton.TabStop = true;
        audioFormatRadioButton.Text = "mp3";
        audioFormatRadioButton.UseVisualStyleBackColor = true;
        // 
        // GetAudioButton
        // 
        GetAudioButton.Location = new Point(703, 69);
        GetAudioButton.Name = "GetAudioButton";
        GetAudioButton.Size = new Size(75, 23);
        GetAudioButton.TabIndex = 3;
        GetAudioButton.Text = "Get Audio";
        GetAudioButton.UseVisualStyleBackColor = true;
        GetAudioButton.Click += GetAudioButton_Click;
        // 
        // progressBar
        // 
        progressBar.Location = new Point(23, 411);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(755, 23);
        progressBar.TabIndex = 4;
        progressBar.Maximum = 100;
        // 
        // statusLabel
        // 
        statusLabel.AutoSize = true;
        statusLabel.BackColor = Color.Transparent;
        statusLabel.Location = new Point(26, 391);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(0, 15);
        statusLabel.TabIndex = 5;
        statusLabel.Click += StatusLabel_Click;
        // 
        // TranscribeButton
        // 
        TranscribeButton.Location = new Point(703, 117);
        TranscribeButton.Name = "TranscribeButton";
        TranscribeButton.Size = new Size(75, 23);
        TranscribeButton.TabIndex = 6;
        TranscribeButton.Text = "Transcribe";
        TranscribeButton.UseVisualStyleBackColor = true;
        TranscribeButton.Click += TranscribeButton_Click;
        // 
        // transcriptTextBox
        // 
        transcriptTextBox.Location = new Point(23, 117);
        transcriptTextBox.Multiline = true;
        transcriptTextBox.Name = "transcriptTextBox";
        transcriptTextBox.ScrollBars = ScrollBars.Vertical;
        transcriptTextBox.Size = new Size(651, 255);
        transcriptTextBox.TabIndex = 7;
        // 
        // GetSummaryButton
        // 
        GetSummaryButton.Location = new Point(703, 166);
        GetSummaryButton.Name = "GetSummaryButton";
        GetSummaryButton.Size = new Size(75, 23);
        GetSummaryButton.TabIndex = 8;
        GetSummaryButton.Text = "Summarize";
        GetSummaryButton.UseVisualStyleBackColor = true;
        GetSummaryButton.Click += GetSummaryButton_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(GetSummaryButton);
        Controls.Add(transcriptTextBox);
        Controls.Add(TranscribeButton);
        Controls.Add(statusLabel);
        Controls.Add(progressBar);
        Controls.Add(GetAudioButton);
        Controls.Add(audioFormatRadioButton);
        Controls.Add(videoSourceTextBox);
        Controls.Add(GetVideoButton);
        Name = "MainForm";
        Text = "Youtube Pod Smart Summary";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button GetVideoButton;
    private TextBox videoSourceTextBox;
    private RadioButton audioFormatRadioButton;
    private Button GetAudioButton;
    private ProgressBar progressBar;
    private Label statusLabel;
    private Button TranscribeButton;
    private TextBox transcriptTextBox;
    private Button GetSummaryButton;
    private System.Windows.Forms.Timer progressTimer;
}
