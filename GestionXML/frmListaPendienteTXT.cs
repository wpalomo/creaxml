﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;

namespace GestionXML
{
    public partial class frmListaPendienteTXT : Form
    {
        public string _path_camino = "";
        public int _id_camino = 0;

        public frmListaPendienteTXT()
        {
            InitializeComponent();
        }

        private void frmListaPendienteTXT_Load(object sender, EventArgs e)
        {
            //cmbEquipos.SelectedIndex = 0;


            //lblCamino.Text = _path_camino;


            //cargo los pdf
            CargaGrid(0);
        }

       

        public void CargaGrid(int _tipo)
        {
            dataGridView1.Rows.Clear();

            DirectoryInfo directory = new DirectoryInfo(@_path_camino);
            FileInfo[] filesPDF = directory.GetFiles("*.PDF");
            FileInfo[] filesXML = directory.GetFiles("*.XML");

            DataTable dtXML = new DataTable();
            dtXML.Columns.Add("nombre");

            string _nombre_xml = "";
            int _cantidadXMLCrear = 0;


            int id_caminos;
            string nombre_produccion_detalle = "";
            DateTime inicio_produccion_detalle;
            DateTime fin_produccion_detalle;
            int id_usuarios_crea;
            string nombre_sustituir = "";
            string _equipo = "";
            DateTime _date_creado_xml = DateTime.Now;
            int _agregados = 0;

            ///busco los xml segun la base de datos
            /// 
            string _parametros = "usuarios.id_usuarios = produccion_detalle.id_usuarios_crea";
            DataTable dtCaminos = AccesoLogica.Select("produccion_detalle.id_caminos, produccion_detalle.nombre_produccion_detalle,produccion_detalle.inicio_produccion_detalle, produccion_detalle.fin_produccion_detalle, produccion_detalle.id_usuarios_crea", "public.produccion_detalle, public.usuarios", _parametros);
            int reg = dtCaminos.Rows.Count;
            if (reg > 0)
            {
                foreach (DataRow renglon in dtCaminos.Rows)
                {
                    id_caminos = Convert.ToInt32(renglon["id_caminos"].ToString());
                    nombre_produccion_detalle = Convert.ToString(renglon["nombre_produccion_detalle"].ToString());
                    inicio_produccion_detalle = Convert.ToDateTime(renglon["inicio_produccion_detalle"].ToString());


                    for (int ii = 0; ii < filesXML.Length; ii++)
                    {
                        _nombre_xml = ((FileInfo)filesXML[ii]).FullName;

                        if (_nombre_xml == nombre_produccion_detalle)
                        {
                            MessageBox.Show("Nombre del XML ->" + _nombre_xml);
                            dtXML.Rows.Add(_nombre_xml);
                            _cantidadXMLCrear++;
                            _date_creado_xml = File.GetLastWriteTime(filesXML[ii].FullName);





                        }

                        if (_tipo == 0)
                        {

                            dataGridView1.Rows.Insert(_agregados, _agregados + 1, _date_creado_xml, nombre_produccion_detalle);
                            _agregados++;

                        }


                        if (_tipo == 2)
                        {
                            _equipo = txtTextoBuscar.Text.ToString();
                            bool b = nombre_produccion_detalle.Contains(_equipo);

                            if (_equipo == "TODOS")
                            {

                                dataGridView1.Rows.Insert(_agregados, _agregados + 1, _date_creado_xml, _nombre_xml);
                                _agregados++;
                            }
                            else if (b)
                            {
                                dataGridView1.Rows.Insert(_agregados, _agregados + 1, _date_creado_xml, _nombre_xml);
                                _agregados++;

                            }
                        }

                    }
                }
            }


        }

        private void txtTextoBuscar_TextChanged(object sender, EventArgs e)
        {
            CargaGrid(2);
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow fila = dataGridView1.CurrentRow;
            string _camino = Convert.ToString(fila.Cells[2].Value); //obtengo el valor de la primer columna


            ////busco el id del indice cabeza
            string _parametros = " id_caminos = '" + _id_camino + "'   ";

            int _id_indice_cabeza = 0;

            DataTable dtCaminos = AccesoLogica.Select("id_indice_cabeza", "indice_cabeza", _parametros);
            int reg = dtCaminos.Rows.Count;
            if (reg > 0)
            {
                foreach (DataRow renglon in dtCaminos.Rows)
                {
                    _id_indice_cabeza = Convert.ToInt32(renglon["id_indice_cabeza"].ToString());
                }
            }


            DialogResult result = MessageBox.Show("Deseas Crear XML de este PDF?", "Crear nuevo XML", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {

                EditorXML Crea = new EditorXML();
                Crea.nombre_pdf = _camino.Replace(".XML", ".pdf");
                Crea._id_indice_cabeza = _id_indice_cabeza;
                Crea.Show();
            }
            if (result == DialogResult.No)
            {
            }

        }
    }
}
