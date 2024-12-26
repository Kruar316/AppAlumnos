using Firebase.Database;
using Firebase.Database.Query;
using LiteDB;
using AlumnosApp.Modelos;
using System.Collections.ObjectModel;

namespace AlumnosApp.Vistas;

public partial class ListarAlumnos : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroalumnos-8406d-default-rtdb.firebaseio.com/");
    public ObservableCollection<Alumno> Lista { get; set; } = new ObservableCollection<Alumno>();
    public ListarAlumnos()
    {
        InitializeComponent();
        BindingContext = this;
        CargarLista();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarLista();
    }

    private async void CargarLista()
    {
        try
        {
            var alumnos = await client.Child("Alumnos").OnceAsync<Alumno>();

            var alumnosActivos = alumnos.Where(e => e.Object.Estado == true).ToList();

            Lista.Clear(); 

            foreach (var alumno in alumnosActivos)
            {
                Lista.Add(new Alumno
                {
                    Id = alumno.Key,
                    PrimerNombre = alumno.Object.PrimerNombre,
                    SegundoNombre = alumno.Object.SegundoNombre,
                    PrimerApellido = alumno.Object.PrimerApellido,
                    SegundoApellido = alumno.Object.SegundoApellido,
                    CorreoElectronico = alumno.Object.CorreoElectronico,
                    Edad = alumno.Object.Edad,
                    Estado = alumno.Object.Estado,
                    Curso = alumno.Object.Curso
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al cargar la lista: " + ex.Message, "OK");
        }
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar.Text.ToLower();

        if (filtro.Length > 0)
        {
            listaCollection.ItemsSource = Lista.Where(x => x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            listaCollection.ItemsSource = Lista;
        }
    }

    private async void NuevoAlumnoBoton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearAlumno());
    }

    private async void editarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var alumno = boton?.CommandParameter as Alumno;

        if (alumno != null && !string.IsNullOrEmpty(alumno.Id))
        {
            await Navigation.PushAsync(new EditarAlumno(alumno.Id));
        }
        else
        {
            await DisplayAlert("Error", "No se pudo obtener la información del alumno", "OK");
        }
    }

    private async void deshabilitarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var alumno = boton?.CommandParameter as Alumno;

        if (alumno == null)
        {
            await DisplayAlert("Error", "No se pudo obtener la información del alumno", "OK");
            return;
        }

        bool confirmacion = await DisplayAlert
            ("Confirmación", $"Está seguro que desea deshabilitar al alumno {alumno.NombreCompleto}", "Sí", "No");

        if (confirmacion)
        {
            try
            {
                alumno.Estado = false;
                await client.Child("Alumnos").Child(alumno.Id).PutAsync(alumno);
                await DisplayAlert("Éxito", $"Se ha deshabilitado correctamente al alumno {alumno.NombreCompleto}", "OK");
                CargarLista();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}