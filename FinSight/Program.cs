using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

//Crear la carpeta Datos y definir la ruta de los archivos

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
            consultarMovimientos();
            break;
        case "4":
            break;
        case "5":
            break;
        case "6":
            Console.WriteLine("Saliendo del programa...");
            break;
        default:
            Console.WriteLine("Opción invalida.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }
} while (opcion != "6");

void InicializarArchivos(string archivoCatalogo, string archivoMovimientos)
{
    if (!File.Exists(archivoCatalogo))
    {
        using (StreamWriter escritor = new StreamWriter(archivoCatalogo, false))
        {
            escritor.WriteLine("Codigo;Nombre;Tipo;Naturaleza");
        }
    }

    if (!File.Exists(archivoMovimientos))
    {
        using (StreamWriter escritor = new StreamWriter(archivoMovimientos, false))
        {
            escritor.WriteLine("Fecha;Cuenta;Debe;Haber;Concepto");
        }
    }
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
    Console.WriteLine("6. Salir");
    Console.WriteLine("========================================");
    Console.Write("Seleccione una opción: ");
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
        string opcion = (Console.ReadLine() ?? string.Empty).Trim();

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
string ObtenerNaturaleza(string tipo)
{
    if (tipo == "Activo" || tipo == "Costo" || tipo == "Gasto")
    {
        return "Deudora";
    }

    return "Acreedora";
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
        lector.ReadLine();
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

                Console.WriteLine($"| {datos[0],-ANCHO_CODIGO} | {nombre,-ANCHO_NOMBRE} | {datos[2],-ANCHO_TIPO} | {datos[3],-ANCHO_NATURALEZA} |");
                hayCuentas = true;
            }
        }
    }
    Console.WriteLine(linea);

    if (!hayCuentas)
        Console.WriteLine("No hay cuentas registradas.");

}
void BuscarCuenta()
{
    Console.Write("\nIngrese el codigo a buscar: ");
    opcion = (Console.ReadLine() ?? string.Empty).Trim();

    string[]? cuenta = ObtenerCuenta(opcion);
    if (cuenta == null)
    {
        Console.WriteLine("\nCuenta no encontrada.");
        return;
    }

    Console.WriteLine("\n===== CUENTA ENCONTRADA =====");
    Console.WriteLine($"| {cuenta[0],-8} | {cuenta[1],-26} | {cuenta[2],-12} | {cuenta[3],-12} |");
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
void EliminarCuenta()
{
    Console.Write("Ingrese el código de la cuenta a eliminar: ");
    string codigo = (Console.ReadLine() ?? string.Empty).Trim();

    string[]? cuenta = ObtenerCuenta(codigo);

    if (cuenta == null)
    {
        Console.WriteLine("\nCuenta no encontrada.");
        return;
    }

    Console.WriteLine($"Cuenta: {cuenta[0]} - {cuenta[1]}");
    Console.Write("¿Confirma la eliminación? (S/N):  ");
    string confirmacion = (Console.ReadLine() ?? string.Empty).Trim().ToUpper();

    if (confirmacion != "S")
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

    if (eliminada)
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
void registrarMovimientos()
{

    Console.WriteLine("\n======== REGISTRO DE MOVIMIENTOS ========");

    string fechaValida = PedirFecha(fechaInicio, fechaFin);
    if (fechaValida == "") return;

    string codigoCuenta = PedirCodigocuenta();
    if (codigoCuenta == "") return;

    string tipo = SeleccionarTipoMovimiento();
    if (tipo == "") return;

    decimal monto = PedirMonto(tipo);
    if (monto <= 0) return;

    string concepto = PedirConcepto();
    if (concepto == "") return;

    decimal debe = (tipo == "Debe") ? monto : 0;
    decimal haber = (tipo == "Haber") ? monto : 0;

    Console.WriteLine("\n========================================");
    Console.WriteLine("          RESUMEN DEL ASIENTO           ");
    Console.WriteLine("========================================");
    Console.WriteLine($"Fecha:      {fechaValida}");
    Console.WriteLine($"Cuenta:     {codigoCuenta}");
    Console.WriteLine($"Tipo:       {tipo}");
    Console.WriteLine($"Monto:      C$ {monto:N2}");
    Console.WriteLine($"Concepto:   {concepto}");
    Console.WriteLine("========================================");

    Console.Write("¿Desea registrar este movimiento? (S/N): ");
    string confirmacion = (Console.ReadLine() ?? string.Empty).Trim().ToUpper();

    if (confirmacion != "S")
    {
        Console.WriteLine("Registro cancelado.");
        return;
    }

    using (StreamWriter escritor = new StreamWriter(archivoMovimientos, true))
    {
        escritor.WriteLine($"{fechaValida};{codigoCuenta};{tipo};{monto};{concepto}");
    }

    Console.WriteLine("\n¡Movimiento registrado con éxito!");

}
string PedirFecha(DateTime fechaInicio, DateTime fechaFin)
{
    Console.Write("Ingrese la fecha del movimiento (dd/mm/yyyy): ");
    string fechaIngresada = (Console.ReadLine() ?? string.Empty).Trim();

    if (DateTime.TryParse(fechaIngresada, out DateTime fecha))
    {
        if (fecha >= fechaInicio && fecha <= fechaFin)
        {
            Console.WriteLine($"Fecha registrada: {fecha.ToString("dd/MM/yyyy")}");
            return fecha.ToString("dd/MM/yyyy");

        }
        Console.WriteLine($"La fecha debe estar entre {fechaInicio.ToString("dd/MM/yyyy")} y {fechaFin.ToString("dd/MM/yyyy")}.");
        return "";
    }

    Console.WriteLine("Fecha inválida.");
    return "";
}
string PedirCodigocuenta()
{

    Console.Write("\nIngrese el código de la cuenta: ");
    string codigo = (Console.ReadLine() ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(codigo) || codigo.Contains(';'))
    {
        Console.WriteLine("Código no válido.");
        return "";
    }

    // Buscamos en el catálogo usando ObtenerCuenta
    string[]? datosCuenta = ObtenerCuenta(codigo);

    if (datosCuenta != null)
    {
        // datosCuenta[0] = Código, datosCuenta[1] = Nombre
        Console.WriteLine($"Cuenta seleccionada: {datosCuenta[0]} - {datosCuenta[1]}");
        return datosCuenta[0];
    }

    Console.WriteLine($"El código '{codigo}' no existe en el catálogo.");
    return "";
}
string SeleccionarTipoMovimiento()
{
    Console.WriteLine("\n Tipo de movimiento");
    Console.WriteLine("1. Debe");
    Console.WriteLine("2. Haber");
    Console.Write("Seleccione una opción: ");
    string opcion = (Console.ReadLine() ?? string.Empty).Trim();

    switch (opcion)
    {
        case "1": return "Debe";
        case "2": return "Haber";
        default:
            Console.WriteLine("Opción inválida.");
            return "";
    }
}
decimal PedirMonto(string tipo)
{
    Console.Write($"Monto ({tipo}): ");
    string entrada = (Console.ReadLine() ?? "").Trim();

    if (string.IsNullOrWhiteSpace(entrada))
    {
        Console.WriteLine("El monto no puede estar vacío.");
        return 0;
    }

    //Quitar comas de cantidades
    string sinComas = entrada.Replace(",", "");

    //Reemplazar punto por coma para el parseo local de C#
    string formatoLocal = sinComas.Replace('.', ',');

    if (!decimal.TryParse(formatoLocal, out decimal monto))
    {
        Console.WriteLine("Monto inválido.");
        return 0;
    }

    if (monto <= 0)
    {
        Console.WriteLine("El monto debe ser mayor a 0.");
        return 0;
    }

    // Validar que no tenga más de 2 decimales
    if (monto != Math.Round(monto, 2))
    {
        Console.WriteLine("El monto no puede tener más de 2 decimales.");
        return 0;
    }

    return monto;
}
string PedirConcepto()
{
    Console.Write("Concepto: ");
    string concepto = (Console.ReadLine() ?? string.Empty).Trim();

    if (concepto == "" || concepto.Contains(';'))
    {
        Console.WriteLine("Concepto inválido.");
        return "";
    }

    return concepto;
}
void consultarMovimientos()
{
    do
    {
        Console.WriteLine("\n======== CONSULTA DE MOVIMIENTOS ========");
        Console.WriteLine("1. Ver todos los movimientos");
        Console.WriteLine("2. Ver movimientos por fecha");
        Console.WriteLine("3. Ver movimientos por cuenta");
        Console.WriteLine("4. Volver");
        Console.WriteLine("=========================================");
        Console.Write("Seleccione una opción: ");
        string opcion = (Console.ReadLine() ?? string.Empty).Trim(); 

    } while (opcion != "4");

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


