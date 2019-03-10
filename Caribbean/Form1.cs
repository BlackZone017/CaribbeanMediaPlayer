using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caribbean.Properties;

namespace Caribbean
{
    public partial class Form1 : Form
    {
        private bool loadingPlayer;
        private bool play = true;

        public Form1()
        {
            InitializeComponent();
            //Color de los botones
            btnPlay.BackColor = Color.Transparent;
            btnNext.BackColor = Color.Transparent;
            btnPrev.BackColor = Color.Transparent;
            //
            loadingPlayer = false;
            axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(axWindowsMediaPlayer1_PlayStateChange);
        }

        //Metodo iniciar el timer
        public void iniciarTiempo()
        {
            timer1.Interval = 1000; //Intervalo de 1s
            timer1.Start();
        }

        //Metodo Abrir un archivo 
        private void Abrir()
        {
            OpenFileDialog abrir = new OpenFileDialog();
            abrir.FileName = "archivo nuevo";
            abrir.Filter = "archivo mp3|*.mp3|archivo mp4|*.mp4|archivo avi|*.avi|archivo mkv|*.mkv";
            abrir.FilterIndex = 1;
            if (abrir.ShowDialog() == DialogResult.OK)
            {
                axWindowsMediaPlayer1.URL = abrir.FileName;
                loadingPlayer = true;
                barMinutos.Value = 0;
                iniciarTiempo();
            }
        }

        //Metodo Reproducir siguiente elemento del listBox
        //Si el elemento seleccionado es el ultimo, al dar click a siguiente cambiar al primer elemento 
        private void Siguiente()
        {
            if (lista.SelectedIndex == lista.Items.Count - 1)
            {
                lista.SelectedIndex = 0;
                axWindowsMediaPlayer1.URL = lista.SelectedItem.ToString();
            }
            else
            {
                lista.SelectedIndex++;
                axWindowsMediaPlayer1.URL = lista.SelectedItem.ToString();
            }

            if (!play)
            {
                btnPlay.Image = Resources.btnPause;
                play = true;
            }

            loadingPlayer = true;
            barMinutos.Value = 0;

        }

        //Metodo Reproducir anterior elemento del listBox
        //Si el elemento seleccionado es el primero, al dar click a anterior cambiar al ultimo elemento 
        private void Anterior()
        {
            if (lista.SelectedIndex <= 0)
            {
                lista.SelectedIndex = lista.Items.Count - 1;
                axWindowsMediaPlayer1.URL = lista.SelectedItem.ToString();
            }
            else
            {
                lista.SelectedIndex--;
                axWindowsMediaPlayer1.URL = lista.SelectedItem.ToString();
            }

            if (!play)
            {
                btnPlay.Image = Resources.btnPause;
                play = true;
            }

            loadingPlayer = true;
            barMinutos.Value = 0;
        }

        //Menu Archivo -> Abrir
        private void abrirCtrlOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Abrir();
        }

        //Menu Acerca De
        private void acercaDeF1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Gouri Ramirez | 2017-5184");
        }

        //Menu Salir
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //Boton Play/Pause
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!play) //Si esta pausado cambiar de icono y poner play
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
                btnPlay.Image = Resources.btnPause;
                play = true;
            }
            else //Si esta en play cambiar de icono y poner pause
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                btnPlay.Image = Resources.btnPlay;
                play = false;
            }
            
        }

        //Boton Anterior
        private void btnPrev_Click(object sender, EventArgs e)
        {
            Anterior();
        }

        //Boton Siguiente
        private void btnNext_Click(object sender, EventArgs e)
        {
            Siguiente();
        }

        //Boton Agregar Archivo a la lista de reproduccion
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Abrir();
            if (!play)
            {
                btnPlay.Image = Resources.btnPause;
                play = true;
            }
            lista.Items.Add(axWindowsMediaPlayer1.URL.ToString());
        }

        //Boton Eliminar Elemento de la lista
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                lista.Items.RemoveAt(lista.SelectedIndex);
                lista.SelectedIndex = 0;
                barMinutos.Value = 0;
            }
            catch (Exception ex) { }
        }

        
        private void lista_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!play)
                {
                    btnPlay.Image = Resources.btnPause;
                    play = true;
                }
                axWindowsMediaPlayer1.URL = lista.SelectedItem.ToString();
                loadingPlayer = true;
            }
            catch (Exception ex){ }
        }

        
        //Controlador del volumen (volumeBar event)
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            volumeBar.Minimum = 0;
            volumeBar.Maximum = 100;
            axWindowsMediaPlayer1.settings.volume = volumeBar.Value;
        }

        //Controlador de la posicion del video (barMinutos)
        private void barMinutos_Scroll(object sender, EventArgs e)
        {
            //adaptar el video al valor del barMinutos
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = barMinutos.Value;
        }

        //
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (loadingPlayer && e.newState == 3)
            {
                //Setear el valor maximo de la barra de minutos como la duracion del video
                barMinutos.Maximum = (int)axWindowsMediaPlayer1.currentMedia.duration;

                loadingPlayer = false;

                //avanzar en 1 la barra
                barMinutos.SmallChange = 1;         
            }
        }

        //Avanzar la barra de Minutos cada segundo y mostrar el tiempo restante del video
        private void timer1_Tick(object sender, EventArgs e)
        {
            double t = Math.Floor(axWindowsMediaPlayer1.currentMedia.duration - axWindowsMediaPlayer1.Ctlcontrols.currentPosition);

            //Convertir los segundos (t) de tipo double a Timespan, y luego a String
            string str = Convert.ToString(TimeSpan.FromSeconds(t));
            
            // Display the time remaining in the current media.
            label3.Text = ("Time remaining: " + str);

            //avanzar el puntero de la barra de minutos conforme avance el video
            barMinutos.Value = (int)axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
        }

    }
}
