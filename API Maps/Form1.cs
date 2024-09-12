using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Windows.Controls;
using System.Threading;

namespace API_Maps
{
    public partial class Form1 : Form
    {
        private const string llaveApi = "API";
        public Form1()
        {
            InitializeComponent();

            Thread coor = new Thread(cicloDatos);
            coor.Start();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        bool ya=false;

        double latitud;
        double longitud;
        bool state;
        bool cicloF=true;
        private void cicloDatos()
        {
            
            while (cicloF)
            {

                state = this.userControl11.getState();
                if (ya == false && state== true)
                {
                    ya = true;
                    latitud = this.userControl11.latitude;
                    longitud = this.userControl11.longitude;
                    MessageBox.Show("Coordenadas: "+latitud+" , "+longitud);
                    while (state == true) ;
                }
                   
            }

        }


        private async Task<double> CalcularDistanciaAsync(double latitud1, double longitud1, double latitud2, double longitud2)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUri = $"https://dev.virtualearth.net/REST/v1/Routes/DistanceMatrix?origins={latitud1},{longitud1}&destinations={latitud2},{longitud2}&travelMode=driving&key={llaveApi}";

                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<DistanceMatrixResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.ResourceSets[0]?.Resources[0]?.Results[0]?.TravelDistance ?? 0;
                }
                else
                {
                    throw new InvalidOperationException($"Error al calcular la distancia: {response.StatusCode}");
                }
            }
        }

        private async void btnCalcularDistancia_Click_1(object sender, EventArgs e)
        {
                                                        //22.151844361388388, -100.97116408283527
            double lati = latitud; //48.8588446; // 22.145123652960574, -101.01551373119145
            double longi = longitud;//2.2943506; // 22.12705240350552, -101.00200708964489
            double tangamangaLat = 22.12705240350552;
            double tangamangaLon = -101.00200708964489;

            double distancia = await CalcularDistanciaAsync(lati, longi, tangamangaLat, tangamangaLon);

            ya = false;
            state = false;
            this.userControl11.state = false;

            MessageBox.Show($"La distancia entre la posicion dada y el parque morales es de aproximadamente {distancia/2} kilómetros.");
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cicloF = false;
        }
    }

    public class DistanceMatrixResult
    {
        public ResourceSet[] ResourceSets { get; set; }
    }

    public class ResourceSet
    {
        public Resource[] Resources { get; set; }
    }

    public class Resource
    {
        public Result[] Results { get; set; }
    }

    public class Result
    {
        public double TravelDistance { get; set; }
    }

    

}
