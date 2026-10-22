//Crear la carpeta Datos y definir la ruta de los archivos
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

string carpetaProyecto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../.."));

string carpetaDatos = Path.Combine(carpetaProyecto, "Datos");

Directory.CreateDirectory(carpetaDatos);

//Inicializacion de los archivos
string archivoCatalogo = Path.Combine(carpetaDatos, "catalogo.csv");
string archivoMovimientos = Path.Combine(carpetaDatos, "movimientos.csv");

//Variable global
string opcion;

//Crear archivos
InicializarArchivos(archivoCatalogo, archivoMovimientos);

//Definir los 15 dias contables de la simulacion
DateTime fechaInicio = new DateTime(2026, 10, 1);
DateTime fechaFin = fechaInicio.AddDays(14);

do
{
    MostrarMenu();

    opcion = (Console.ReadLine() ?? string.Empty).Trim();

    switch (opcion)
    {
        case "1":
            administrarCatalogo();
            break;
        case "2":
            registrarMovimientos();
            break;
        case "3":
            break;
        case "4":
            break;
        case "5":
            break;
        case "6":
            break;
        case "7":
            break;
        case "8":
            break;
        case "9":
            break;
        default:
            Console.WriteLine("Opción invalida.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }
} while (opcion != "9");

void registrarMovimientos()
{

    Console.WriteLine("\n======== REGISTRO DE MOVIMIENTOS ========");

    string fechaValida = PedirFecha(fechaInicio, fechaFin);

    if (fechaValida == "0")
    {
        Console.WriteLine("Registro de movimientos cancelado.");
        Console.WriteLine("Volviendo al menu principal...");
        return;
    }
}

string PedirFecha(DateTime fechaInicio, DateTime fechaFin)
{
    while (true) 
    { 
        Console.Write("Ingrese la fecha del movimiento (dd/mm/yyyy) o '0' para cancelar: ");
        string fechaIngresada = (Console.ReadLine() ?? string.Empty).Trim();

        if (fechaIngresada == "0") return "0";
        

        if (DateTime.TryParse(fechaIngresada, out DateTime fecha))
        {
            if (fecha >= fechaInicio && fecha <= fechaFin)
            {
                Console.WriteLine($"Fecha registrada: {fecha.ToString("dd/MM/yyyy")}");
                return fecha.ToString("dd/MM/yyyy");

            }

            Console.WriteLine($"La fecha debe estar entre {fechaInicio.ToString("dd/MM/yyyy")} y {fechaFin.ToString("dd/MM/yyyy")}.");
            continue;
        }

        Console.WriteLine("Fecha inválida. Intente nuevamente.");
    }
}
void administrarCatalogo()
{
    do
    {
        Console.WriteLine("\n====== ADMINISTRACIÓN DEL CATÁLOGO ======");
        Console.WriteLine("1. Registrar cuenta");
        Console.WriteLine("2. Consultar todas las cuentas");
        Console.WriteLine("3. Buscar cuenta por código");
        Console.WriteLine("4. Modificar cuenta");
        Console.WriteLine("5. Eliminar cuenta");
        Console.WriteLine("6. Volver");
        Console.WriteLine("=========================================");
        Console.Write("Seleccione una opción: ");
        opcion = (Console.ReadLine() ?? string.Empty).Trim(); 

        switch (opcion)
        {
            case "1":
                RegistrarCuenta();
                break;
            case "2":
                ConsultarCuentas();
                break;
            case "3":
                BuscarCuenta();
                break;
            case "4":
                ModificarCuenta();
                break;
            case "5":
                EliminarCuenta();
                break;
            case "6":
                Console.WriteLine("Regresando al menú principal...");
                break;
            default:
                Console.WriteLine("Opción invalida.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
        }
    } while (opcion != "6");
}
void EliminarCuenta()
{
    Console.Write("Ingrese el código de la cuenta a eliminar: ");
    string codigo = (Console.ReadLine() ?? string.Empty).Trim();

    string[]? cuenta = ObtenerCuenta(opcion);

    if (cuenta == null)
    {
        Console.WriteLine("\nCuenta no encontrada.");
        return;
    }

    Console.WriteLine($"Cuenta: {cuenta[0]} - {cuenta[1]}");
    Console.Write("¿Confirma la eliminación? (S/N):  ");
    string confirmacion = (Console.ReadLine() ?? string.Empty).Trim().ToUpper();

    if(confirmacion != "S")
    {
        Console.WriteLine("Eliminación cancelada.");
        return;
    }

    string temporal = archivoCatalogo + ".tmp";
    bool eliminada = false;

    using (StreamReader lector = new StreamReader(archivoCatalogo, false))
    {
        using (StreamWriter escritor = new StreamWriter(temporal, false))
        {
            string? linea;
            while ((linea = lector.ReadLine()) != null)
            {
                string[] datos = linea.Split(';');
                if (datos.Length == 4 && datos[0] == codigo)
                {
                    eliminada = true;
                    
                }
                else
                {
                    escritor.WriteLine(linea);
                }
            }
        }
    }

    if(eliminada)
    {
        File.Move(temporal, archivoCatalogo, true);
        Console.WriteLine("Cuenta eliminada correctamente.");
    }
    else
    {
        File.Delete(temporal);
        Console.WriteLine("No se pudo eliminar la cuenta.");
    }
}
void ModificarCuenta()
{
    Console.Write("Ingrese el codigo de la cuenta a modificar: ");
    opcion = (Console.ReadLine() ?? string.Empty).Trim();

    string[]? cuenta = ObtenerCuenta(opcion);

    if (cuenta == null)
    {
        Console.WriteLine("\nCuenta no encontrada.");
        return;
    }

    Console.WriteLine($"Nombre actual: {cuenta[1]}");
    Console.Write("Nuevo nombre (Enter para guardar): ");
    string Nombre = (Console.ReadLine() ?? string.Empty).Trim();

    if (Nombre.Contains(';'))
    {
        Console.WriteLine("El nombre no puede contener el carácter ';'.");
        return;
    }
       
    if (Nombre == "")
    {
        Nombre = cuenta[1];
        Console.WriteLine("No se realizaron cambios.");
        return;
    }

    Console.WriteLine($"Tipo actual: {cuenta[2]}");

    Console.WriteLine("Desea cambiar el tipo? (S/N): ");
    string respuesta = (Console.ReadLine() ?? string.Empty).Trim().ToUpper();
    string tipo = cuenta[2];

    if (respuesta == "S")
    {
        tipo = SeleccionarTipoCuenta();
        if (tipo == "")
            return;
    }
    else if (respuesta != "N")
    {
        Console.WriteLine("Respuesta no válida.");
        return;
    }

    string naturaleza = ObtenerNaturaleza(tipo);

    // Actualizar la cuenta en el archivo

    string temporal = archivoCatalogo + ".tmp";
    bool modificada = false;

    using (StreamReader lector = new StreamReader(archivoCatalogo))
    {
        using (StreamWriter escritor = new StreamWriter(temporal, false))
        {
            string? linea;
            while ((linea = lector.ReadLine()) != null)
            {
                string[] datos = linea.Split(';');
                if (datos.Length == 4 && datos[0] == opcion)
                {
                    escritor.WriteLine($"{opcion};{Nombre};{tipo};{naturaleza}");
                    modificada = true;
                }
                else
                {
                    escritor.WriteLine(linea);
                }
            }
        }  
    
    }

    if (modificada)
    {
        File.Move(temporal, archivoCatalogo, true);
        Console.WriteLine("Cuenta modificada correctamente.");
    }
    else
    {
        File.Delete(temporal);
        Console.WriteLine("No se realizaron cambios.");
    }
}
void BuscarCuenta()
{
    Console.Write("\nIngrese el codigo a buscar: ");
    opcion = (Console.ReadLine() ?? string.Empty).Trim();

    string []? cuenta = ObtenerCuenta(opcion);
    if (cuenta == null)
    {
        Console.WriteLine("\nCuenta no encontrada.");
        return;
    }

    Console.WriteLine("\n===== CUENTA ENCONTRADA =====");
    Console.WriteLine($"| {cuenta[0],-8} | {cuenta[1],-26} | {cuenta[2],-12} | {cuenta[3],-12} |");
}
void ConsultarCuentas()
{
    Console.WriteLine("\n========================= CATALOGO DE CUENTAS =========================");
    const int ANCHO_CODIGO = 8;
    const int ANCHO_NOMBRE = 26;
    const int ANCHO_TIPO = 12;
    const int ANCHO_NATURALEZA = 12;

    // Longitud de la linea separadora
    string linea = new string('-', ANCHO_CODIGO + ANCHO_NOMBRE + ANCHO_TIPO + ANCHO_NATURALEZA + 9);
    Console.WriteLine(linea);

    //mostrar encabezado
    Console.WriteLine($"| {"Codigo",-ANCHO_CODIGO} | {"Nombre",-ANCHO_NOMBRE} | {"Tipo",-ANCHO_TIPO} | {"Naturaleza",-ANCHO_NATURALEZA} |");
    Console.WriteLine(linea);

    bool hayCuentas = false;

    using (StreamReader lector = new StreamReader(archivoCatalogo))
    {
        string? lineaCuenta;
        while ((lineaCuenta = lector.ReadLine()) != null)
        {
            string[] datos = lineaCuenta.Split(';');
            if (datos.Length == 4)
            {
                // Truncar nombre si es demasiado largo
                string nombre = datos[1];
                if (nombre.Length > ANCHO_NOMBRE)
                {
                    nombre = nombre.Substring(0, ANCHO_NOMBRE - 3) + "...";
                }

                Console.WriteLine($"| {datos[0],-ANCHO_CODIGO} | {datos[1],-ANCHO_NOMBRE} | {datos[2],-ANCHO_TIPO} | {datos[3],-ANCHO_NATURALEZA} |");
                hayCuentas = true;
            }
        }
    }
    Console.WriteLine(linea);

    if (!hayCuentas)
        Console.WriteLine("No hay cuentas registradas.");
    
}
void RegistrarCuenta()
{
    Console.WriteLine("\n===== REGISTRAR CUENTA =====");
    Console.Write("Codigo: ");
    string codigo = (Console.ReadLine() ?? string.Empty).Trim();

    if (codigo == "" || codigo.Contains(';'))
    {
        Console.WriteLine("Código inválido");
        return;
    }

    if (ObtenerCuenta(codigo) != null)
    {
        Console.WriteLine("El código ya existe.");
        return;
    }

    Console.Write("Nombre: ");
    string nombre = (Console.ReadLine() ?? string.Empty).Trim();

    if (nombre == "" || nombre.Contains(';'))
    {
        Console.WriteLine("Nombre inválido.");
        return;
    }

    string tipo = SeleccionarTipoCuenta();
    if (tipo == "")
        return;

    string naturaleza = ObtenerNaturaleza(tipo);

    using (StreamWriter escritor = new StreamWriter(archivoCatalogo, true))
    {
        escritor.WriteLine($"{codigo};{nombre};{tipo};{naturaleza}");
    }

    Console.WriteLine("Cuenta registrada correctamente.");
}
string ObtenerNaturaleza(string tipo)
{
    if (tipo == "Activo" || tipo == "Costo" || tipo == "Gasto")
    {
        return "Deudora";
    }

    return "Acreedora";
}
string SeleccionarTipoCuenta()
{
    Console.WriteLine("\nTipo de cuenta:");
    Console.WriteLine("1. Activo");
    Console.WriteLine("2. Pasivo");
    Console.WriteLine("3. Patrimonio");
    Console.WriteLine("4. Ingreso");
    Console.WriteLine("5. Costo");
    Console.WriteLine("6. Gasto");
    Console.Write("Ingrese el tipo de cuenta: ");
    string opcion = Console.ReadLine() ?? "";

    switch (opcion)
    {
        case "1": return "Activo";
        case "2": return "Pasivo";
        case "3": return "Patrimonio";
        case "4": return "Ingreso";
        case "5": return "Costo";
        case "6": return "Gasto";
        default:
            Console.WriteLine("Tipo no válido.");
            return "";
    }
}
string[]? ObtenerCuenta(string codigo)
{
    using (StreamReader lector = new StreamReader(archivoCatalogo))
    {
        string? linea;

        while ((linea = lector.ReadLine()) != null)
        {
            string[] datos = linea.Split(';');

            if (datos.Length == 4 && datos[0] == codigo)
            {
                return datos;
            }
        }
    }
    return null;
}
void MostrarMenu()
{
    Console.WriteLine("\n========================================");
    Console.WriteLine("            SISTEMA CONTABLE            ");
    Console.WriteLine("========================================");
    Console.WriteLine("1. Administrar catálogo de cuentas");
    Console.WriteLine("2. Registrar movimiento contable");
    Console.WriteLine("3. Consultar catálogo de cuentas");
    Console.WriteLine("4. Consultar movimientos ");
    Console.WriteLine("5. Estado de Resultados");
    Console.WriteLine("6. Estado de Situación Financiera");
    Console.WriteLine("7. Estado de Cambios en el Patrimonio");
    Console.WriteLine("8. Estado de Flujos de Efectivo");
    Console.WriteLine("9. Salir");
    Console.WriteLine("========================================");
    Console.Write("Seleccione una opción: ");
}
void InicializarArchivos(string archivoCatalogo, string archivoMovimientos)
{
    if (!File.Exists(archivoCatalogo))
    {
        using(StreamWriter escritor = new StreamWriter(archivoCatalogo, false))
        {
            escritor.WriteLine("Codigo;Nombre;Tipo;Naturaleza");
        }
    }

    if (!File.Exists(archivoMovimientos))
    {
        using (StreamWriter escritor = new StreamWriter(archivoMovimientos, false))
        {
            escritor.WriteLine("Fecha;CodigoCuenta;Descripcion;Debe;Haber");
        }
    }
}
