namespace NetroLogger5_Azure
{
    partial class NetroLogger5
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NewFiles = new System.IO.FileSystemWatcher();
            this.NewFiles2 = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.NewFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NewFiles2)).BeginInit();
            // 
            // NewFiles
            // 
            this.NewFiles.EnableRaisingEvents = true;
            // 
            // NewFiles2
            // 
            this.NewFiles2.EnableRaisingEvents = true;
            // 
            // NetroLogger5
            // 
            this.ServiceName = "Service1";
            ((System.ComponentModel.ISupportInitialize)(this.NewFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NewFiles2)).EndInit();

        }

        #endregion

        private System.IO.FileSystemWatcher NewFiles;
        private System.IO.FileSystemWatcher NewFiles2;
    }
}
