namespace campoMinato
{
    public partial class Form1 : Form
    {
        const int DIM = 15 * 15;
        Button[] pulsanti = new Button[DIM];
        int bombe, campi;
        Button rigioca;
        Label messagio;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) //passa fra tutti i pulsanti e svuotali
        {
            Random rnd = new Random();
            int riga, colonna;
            int dim = (int)(Math.Sqrt(DIM));
            bombe = 0;
            campi = DIM;

            this.Width = 50 * dim + 18;
            this.Height = 50 * dim + 48;

            if (pulsanti[0] != null)
            {
                for (int i = 0; i < DIM; i++)
                {
                    pulsanti[i].Dispose();
                }
                rigioca.Dispose();
                messagio.Dispose();
            }


            for (riga = 0; riga < dim; riga++)
                for (colonna = 0; colonna < dim; colonna++)
                {
                    pulsanti[riga * dim + colonna] = new Button();
                    pulsanti[riga * dim + colonna].Parent = this;
                    pulsanti[riga * dim + colonna].Width = 50;
                    pulsanti[riga * dim + colonna].Height = 50;
                    pulsanti[riga * dim + colonna].Top = riga * 50;
                    pulsanti[riga * dim + colonna].Left = colonna * 50;
                    pulsanti[riga * dim + colonna].FlatStyle = FlatStyle.Flat;
                    pulsanti[riga * dim + colonna].FlatAppearance.BorderSize = 0;
                    if (colonna % 2 == 0 && riga % 2 == 0 || colonna % 2 == 1 && riga % 2 == 1) { pulsanti[riga * dim + colonna].BackColor = Color.Green; }
                    else { pulsanti[riga * dim + colonna].BackColor = Color.LightGreen; }
                    if (bombe < 100)
                        pulsanti[riga * dim + colonna].Tag = rnd.Next(-5, 2); //<=0 = senza bomba, 1 = con bomba
                    else
                        pulsanti[riga * dim + colonna].Tag = 0;
                    if ((int)(pulsanti[riga * dim + colonna].Tag) == 1)
                    {
                        bombe++;
                        //pulsanti[riga * dim + colonna].BackColor = Color.Red;
                    }
                    pulsanti[riga * dim + colonna].MouseUp += controlla_Vicini;
                }
            this.Text = "Moretti Ettore 4IF; Campo minato bombe: " + bombe + " zone rimanenti: " + campi;
        }
        private void controlla_Vicini(object sender, EventArgs e) //guarda se il pulsante stesso è una bomba
        {                                                         //se non lo è guarda gli 8 vicini
            Button clickedButton = (Button)sender;
            int index = Array.IndexOf(pulsanti, clickedButton);
            MouseEventArgs me = (MouseEventArgs)e;
            
            if (me.Button == MouseButtons.Right)
            {
                if (clickedButton.BackColor == Color.Brown)
                {
                    if (index % 2 == 0)
                    {
                        clickedButton.BackColor = Color.Green;

                    }
                    else
                    {
                        clickedButton.BackColor = Color.LightGreen;
                    }
                    if ((int)(clickedButton.Tag) == 4)
                    {
                        clickedButton.Tag = 1;
                    }
                    else
                    {
                        clickedButton.Tag = 0;
                    }
                }
                else
                {
                    clickedButton.BackColor = Color.Brown;
                    if ((int)(clickedButton.Tag) == 1)
                    {
                        clickedButton.Tag = 4;
                    }
                    else
                    {
                        clickedButton.Tag = 3;
                    }
                }

            }
            
            else
            {
                int dim = (int)Math.Sqrt(DIM);
                int riga = index / dim;
                int colonna = index % dim;

                int[] righeAdiacenti = { -1, -1, -1, 0, 0, 1, 1, 1 };
                int[] colonneAdiacenti = { -1, 0, 1, -1, 1, -1, 0, 1 };

                int bombeVicine = 0;
                if ((int)(clickedButton.Tag) <= 0)
                {
                    clickedButton.BackColor = Color.Gray;
                    clickedButton.Tag = 3;          //fa si che non avvenga una ricorsione illimitata
                    clickedButton.MouseUp -= controlla_Vicini;
                    campi--;
                    for (int i = 0; i < righeAdiacenti.Length; i++)
                    {
                        int rigaAdiacente = riga + righeAdiacenti[i];
                        int colonnaAdiacente = colonna + colonneAdiacenti[i];

                        if (rigaAdiacente >= 0 && rigaAdiacente < dim && colonnaAdiacente >= 0 && colonnaAdiacente < dim)
                        {
                            int adiacentIndex = rigaAdiacente * dim + colonnaAdiacente;
                            if ((int)pulsanti[adiacentIndex].Tag == 1 || (int)pulsanti[adiacentIndex].Tag == 4)
                            {
                                bombeVicine++;
                            }
                        }
                    }
                    if (bombeVicine != 0)
                        clickedButton.Text = bombeVicine.ToString();
                    else
                    {
                        for (int i = 0; i < righeAdiacenti.Length; i++)
                        {
                            int rigaAdiacente = riga + righeAdiacenti[i];
                            int colonnaAdiacente = colonna + colonneAdiacenti[i];

                            if (rigaAdiacente >= 0 && rigaAdiacente < dim && colonnaAdiacente >= 0 && colonnaAdiacente < dim)
                            {
                                int adiacentIndex = rigaAdiacente * dim + colonnaAdiacente;
                                if ((int)pulsanti[adiacentIndex].Tag <= 0)
                                {
                                    controlla_Vicini(pulsanti[adiacentIndex], new MouseEventArgs(MouseButtons.None,0,0,0,0));
                                }
                            }
                        }
                    }
                }
                else if ((int)(clickedButton.Tag) == 1)
                {                                       //sconfitta
                    for (int i = 0; i < DIM; i++)
                    {
                        if ((int)(pulsanti[i].Tag) == 1)
                            pulsanti[i].BackColor = Color.Red;
                    }
                    schermata_Finale("HAI PERSO");
                }
                if (campi == bombe)
                {                                       //vittoria
                    schermata_Finale("HAI VINTO");
                }
                this.Text = "Moretti Ettore 4IF; Campo minato bombe: " + bombe + " zone rimanenti: " + campi;
            }
        }

        private void schermata_Finale(String mess)
        {
            for (int i = 0; i < DIM; i++)
            {
                pulsanti[i].Enabled = false;
            }
            messagio = new Label();
            messagio.Text = mess;
            messagio.ForeColor = Color.Black;
            messagio.BackColor = Color.BurlyWood;
            messagio.Font = new Font("Arial", 24);
            messagio.Location = new Point((this.Width - 18) / 2 - (messagio.Width) - 22, (this.Height - 48) / 2 - messagio.Height);
            messagio.AutoSize = true;
            this.Controls.Add(messagio);
            messagio.BringToFront();

            rigioca = new Button(); // bottone per rigiocare
            rigioca.Text = "Rigioca";
            rigioca.ForeColor = Color.Black;
            rigioca.BackColor = Color.BurlyWood;
            rigioca.Font = new Font("Arial", 18);
            rigioca.Location = new Point((this.Width - 18) / 2 - 60, (this.Height - 48) / 2 + 25);
            rigioca.Click += Form1_Load;
            rigioca.FlatStyle = FlatStyle.Flat;
            rigioca.AutoSize = true;
            rigioca.FlatAppearance.BorderSize = 0;
            this.Controls.Add(rigioca);
            rigioca.BringToFront();
        }
    }
}
