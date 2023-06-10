using LegalJuris.Dal;
using LegalJuris.Model;
using System;
using System.Linq;
using System.Windows.Forms;


namespace LegalJuris.Processo
{
    public partial class EditProcessoForms : Form
    {
        public Action Action { get; set; }
        //public Contexto Contexto { get; set; }
        public ProcessoModel Processo { get; set; }

        public EditProcessoForms(Action action, Int32? processoId = null)
        {
            InitializeComponent();

            Init();
            Action = action;

            if (Action == Action.Edit && processoId != null)
            {
                Processo = MainWindow.Contexto.ObjetoProcesso.Find(processoId);

                comboCliente.SelectedItem = comboCliente.Items.OfType<ClienteModel>().FirstOrDefault(cliente1 => cliente1.ClienteId == Processo.Caso.ClienteId);
                comboCaso.SelectedItem = comboCaso.Items.OfType<CasoModel>().FirstOrDefault(caso1 => caso1.ClienteId == Processo.CasoId);
                comboVara.SelectedItem = comboVara.Items.OfType<VaraModel>().FirstOrDefault(vara1 => Processo.Comarca != null && Processo.Comarca.Foro != null && vara1.VaraId == Processo.Comarca.Foro.VaraId);
                comboForo.SelectedItem = comboForo.Items.OfType<ForoModel>().FirstOrDefault(foro1 => Processo.Comarca != null && foro1.ForoId == Processo.Comarca.ForoId);
                comboComarca.SelectedItem = comboComarca.Items.OfType<ComarcaModel>().FirstOrDefault(foro1 => foro1.ComarcaId == Processo.ComarcaId);
                
                comboResponsavel.SelectedItem = comboResponsavel.Items.OfType<ResponsavelModel>().FirstOrDefault(responsavel1 => responsavel1.ResponsavelId == Processo.ResponsavelId);
                numeroProcesso.Text = Processo.NumeroProcesso;
            }
            else 
            {
                comboCaso.Enabled = false;
                comboForo.Enabled = false;
                comboComarca.Enabled = false;
            }
        }

        private void Init()
        {
            var clientes = MainWindow.Contexto.ObjetoCliente.ToArray();
            var casos = MainWindow.Contexto.ObjetoCaso.ToArray();
            var comarcas = MainWindow.Contexto.ObjetoComarca.ToArray();
            var foros = MainWindow.Contexto.ObjetoForo.ToArray();
            var varas = MainWindow.Contexto.ObjetoVara.ToArray();
            var responsaveis = MainWindow.Contexto.ObjetoResponsavel.ToArray();

            comboCliente.Items.AddRange(clientes);
            comboCliente.DisplayMember = "ClienteNome";
            comboCliente.ValueMember = "ClienteId";

            comboCaso.Items.AddRange(casos);
            comboCaso.DisplayMember = "CasoNome";
            comboCaso.ValueMember = "CasoeId";

            comboComarca.Items.AddRange(comarcas);
            comboComarca.DisplayMember = "ComarcaNome";
            comboComarca.ValueMember = "ComarcaId";

            comboForo.Items.AddRange(foros);
            comboForo.DisplayMember = "ForoNome";
            comboForo.ValueMember = "ForoId";

            comboVara.Items.AddRange(varas);
            comboVara.DisplayMember = "VaraNome";
            comboVara.ValueMember = "VaraId";

            comboResponsavel.Items.AddRange(responsaveis);
            comboResponsavel.DisplayMember = "NomeResponsavel";
            comboResponsavel.ValueMember = "ResonsavelId";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var caso = comboCaso.SelectedItem as CasoModel;
            var comarca = comboComarca.SelectedItem as ComarcaModel;
            var responsavel = comboResponsavel.SelectedItem as ResponsavelModel;
            var numeroProcessoValue = numeroProcesso.Text.Trim();

            try
            {
                if (Action == Action.Insert)
                {
                    var processo = new ProcessoModel()
                    {
                        CasoId = caso.CasoId,
                        ComarcaId = comarca.ComarcaId,
                        NumeroProcesso = numeroProcessoValue,
                        ResponsavelId = responsavel.ResponsavelId,
                    };

                    MainWindow.Contexto.ObjetoProcesso.Add(processo);
                    MainWindow.Contexto.SaveChanges();
                    MessageBox.Show("Processo incluído com sucesso.", "Confirmação", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    Processo.CasoId = caso.CasoId;
                    Processo.ComarcaId = comarca.ComarcaId;
                    Processo.NumeroProcesso = numeroProcessoValue;
                    Processo.ResponsavelId = responsavel.ResponsavelId;

                    MainWindow.Contexto.SaveChanges();

                    MessageBox.Show("Processo editado com sucesso.", "Editar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                Close();

                var gridProcessForms = Application.OpenForms.OfType<GridProcessoForms>();
                foreach (var gridProcessForm in gridProcessForms) gridProcessForm.Init();
            } 
            catch 
            {
                MessageBox.Show("Erro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cliente = comboCliente.SelectedItem as ClienteModel;
            
            if (cliente != null) 
            {
                var casos = MainWindow.Contexto.ObjetoCaso
                    .Where(caso1 => caso1.ClienteId == cliente.ClienteId).ToArray();
                comboCaso.Items.Clear();
                comboCaso.Text = null;
                comboCaso.Items.AddRange(casos);
                comboCaso.Enabled = true;
            }
        }

        private void comboForo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var foro = comboForo.SelectedItem as ForoModel;

            if (foro != null)
            {
                var comarcas = MainWindow.Contexto.ObjetoComarca
                    .Where(comarca1 => comarca1.ForoId == foro.ForoId).ToArray();
                comboComarca.Items.Clear();
                comboComarca.Text = null;
                comboComarca.Items.AddRange(comarcas);
                comboComarca.Enabled = true;
            }

        }

        private void comboVara_SelectedIndexChanged(object sender, EventArgs e)
        {
            var vara = comboVara.SelectedItem as VaraModel;

            if (vara != null)
            {
                var foros = MainWindow.Contexto.ObjetoForo
                    .Where(foro1 => foro1.VaraId == vara.VaraId).ToArray();
                comboForo.Items.Clear();
                comboForo.Text = null;
                comboForo.Items.AddRange(foros);
                comboForo.Enabled = true;

                comboComarca.Items.Clear();
                comboComarca.Text = null;                
                comboComarca.Enabled = false;
            }
        }
    }
}
