   

//Crear la carpeta Datos y definir la ruta de los archivos

string carpetaProyecto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../.."));

string carpetaDatos = Path.Combine(carpetaProyecto, "Datos");

string archivoLibroMayor = Path.Combine(carpetaDatos, "libro_mayor.csv");

string archivoEstadoResultados = Path.Combine(carpetaDatos, "estado_resultados.csv");

string archivoBalanceGeneral = Path.Combine(carpetaDatos, "balance_general.csv");

Directory.CreateDirectory(carpetaDatos);

//Inicializacion de los archivos
string archivoCatalogo = Path.Combine(carpetaDatos, "cuentas.csv");
string archivoMovimientos = Path.Combine(carpetaDatos, "movimientos.csv");

//Variable global
string opcion;

// Libro mayor (listas paralelas)
List<string> mayorCodigo = new List<string>();
List<string> mayorNombre = new List<string>();
List<string> mayorTipo = new List<string>();
List<string> mayorNaturaleza = new List<string>();
List<decimal> mayorDebe = new List<decimal>();
List<decimal> mayorHaber = new List<decimal>();
List<decimal> mayorSaldo = new List<decimal>();

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
            ConsultarCuentas();
            break;
        case "4":
            consultarMovimientos();
            break;
        case "5":
            EstadoResultados();
            break;
        case "6":
            EstadoSituacionFinanciera();
            break;
        case "7":
            Console.WriteLine("Saliendo del programa...");
            break;
        default:
            Console.WriteLine("Opción invalida.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }
} while (opcion != "7");

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
    Console.WriteLine("6. Estado de Situación Financiera");
    Console.WriteLine("7. Salir");
    Console.WriteLine("========================================");
    Console.Write("Seleccione una opción: ");
}
void administrarCatalogo()
{
    string opcion = "";
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
    string codigo = (Console.ReadLine() ?? string.Empty).Trim();

    string[]? cuenta = ObtenerCuenta(codigo);

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
                if (datos.Length == 4 && datos[0] == codigo)
                {
                    escritor.WriteLine($"{codigo};{Nombre};{tipo};{naturaleza}");
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
        escritor.WriteLine($"{fechaValida};{codigoCuenta};{debe};{haber};{concepto}");
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
        string cuentaJunta = $"{datosCuenta[0]} - {datosCuenta[1]}";
        Console.WriteLine($"Cuenta seleccionada: {cuentaJunta}");
        return cuentaJunta;
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
    string opcion = "";

    do
    {
        Console.WriteLine("\n======== CONSULTA DE MOVIMIENTOS ========");
        Console.WriteLine("1. Consultar todos los movimientos");
        Console.WriteLine("2. Consultar movimientos por fecha");
        Console.WriteLine("3. Consultar movimientos por cuenta");
        Console.WriteLine("4. Volver");
        Console.WriteLine("=========================================");
        Console.Write("Seleccione una opción: ");
        opcion = (Console.ReadLine() ?? string.Empty).Trim();

        switch (opcion)
        {
            case "1":
                mostrarLosMovimientos();
                break;
            case "2":
                mostrarMovimientosPorFecha();
                break;
            case "3":
                mostrarMovimientosPorCuenta();
                break;
            case "4":
                Console.WriteLine("Regresando al menú principal...");
                break;
            default:
                Console.WriteLine("Opción invalida.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }


        } while (opcion != "4");
}
List<string[]> LeerTodosLosMovimientos()
{
    List<string[]> lista = new List<string[]>();

    if (!File.Exists(archivoMovimientos)) return lista;

    using (StreamReader lector = new StreamReader(archivoMovimientos))
    {
        //descartamos la primera linea de encabezado
        lector.ReadLine();

        string? linea;
        while ((linea = lector.ReadLine()) != null)
        {
            string[] datos = linea.Split(';');
            if (datos.Length == 5)
            {
                lista.Add(datos);
            }
        }
    }
    return lista;
}
(int inicio, int fin) BusquedaBinariaPorFecha(List<string[]> movimientos, DateTime fechaBuscada)
{
    int izquierda = 0;
    int derecha = movimientos.Count - 1;
    int medioEncontrado = -1;

    while (izquierda <= derecha)
    {
        int medio = izquierda + (derecha - izquierda) / 2;

        if (DateTime.TryParse(movimientos[medio][0], out DateTime fechaMedio))
        {
            if (fechaMedio.Date == fechaBuscada.Date)
            {
                medioEncontrado = medio;
                break;
            }
            else if (fechaMedio.Date < fechaBuscada.Date)
            {
                izquierda = medio + 1;
            }
            else
            {
                derecha = medio - 1;
            }
        }
    }

    if (medioEncontrado == -1) return (-1, -1);

    int inicio = medioEncontrado;
    while (inicio > 0 && DateTime.TryParse(movimientos[inicio - 1][0], out DateTime fPrev) && fPrev.Date == fechaBuscada.Date)
    {
        inicio--;
    }

    int fin = medioEncontrado;
    while (fin < movimientos.Count - 1 && DateTime.TryParse(movimientos[fin + 1][0], out DateTime fNext) && fNext.Date == fechaBuscada.Date)
    {
        fin++;
    }

    return (inicio, fin);
}
void MostrarTablaMovimientos(List<string[]> movimientos, string titulo)
{
    if (movimientos == null || movimientos.Count == 0)
    {
        Console.WriteLine($"\nNo se encontraron movimientos para: {titulo}");
        return;
    }

    const int ANCHO_FECHA = 10;
    const int ANCHO_CUENTA = 22;
    const int ANCHO_DEBE = 14;
    const int ANCHO_HABER = 14;
    const int ANCHO_CONCEPTO = 25;

    string linea = new string('-', ANCHO_FECHA + ANCHO_CUENTA + ANCHO_DEBE + ANCHO_HABER + ANCHO_CONCEPTO + 13);

    Console.WriteLine($"\n================================ {titulo.ToUpper()} ================================");
    Console.WriteLine(linea);
    Console.WriteLine($"| {"Fecha",-ANCHO_FECHA} | {"Cuenta",-ANCHO_CUENTA} | {"Debe",-ANCHO_DEBE} | {"Haber",-ANCHO_HABER} | {"Concepto",-ANCHO_CONCEPTO} |");
    Console.WriteLine(linea);

    foreach (var datos in movimientos)
    {
        if (datos.Length == 5)
        {
            decimal.TryParse(datos[2], out decimal debe);
            decimal.TryParse(datos[3], out decimal haber);

            string Debe = $"C$ {debe:N2}";
            string Haber = $"C$ {haber:N2}";
            
            //Truncar la cuenta si es demasiado larga
            string cuenta = datos[1];
            if (cuenta.Length > ANCHO_CUENTA)
            {
                cuenta = cuenta.Substring(0, ANCHO_CUENTA - 3) + "...";
            }

            //Truncar concepto si es demasiado largo
            string concepto = datos[4];
            if (concepto.Length > ANCHO_CONCEPTO)
            {
                concepto = concepto.Substring(0, ANCHO_CONCEPTO - 3) + "...";
            }

            Console.WriteLine($"| {datos[0],-ANCHO_FECHA} | {datos[1],-ANCHO_CUENTA} | {Debe,ANCHO_DEBE} | {Haber,ANCHO_HABER} | {concepto,-ANCHO_CONCEPTO} |");
        }
    }

    Console.WriteLine(linea);
}
void mostrarLosMovimientos()
{
    List<string[]> lista = LeerTodosLosMovimientos();
    MostrarTablaMovimientos(lista, "TODOS LOS MOVIMIENTOS");
}
void mostrarMovimientosPorFecha() 
{
    string fecha = PedirFecha(fechaInicio, fechaFin);
    if (fecha == "") return;

    List<string[]> todos = LeerTodosLosMovimientos();
    var (inicio, fin) = BusquedaBinariaPorFecha(todos, DateTime.Parse(fecha));

    if (inicio == -1)
    {
        Console.WriteLine($"\nNo se encontraron movimientos para la fecha {fecha}.");
        return;
    }

    List<string[]> resultado = todos.GetRange(inicio, fin - inicio + 1);
    MostrarTablaMovimientos(resultado, $"MOVIMIENTOS DEL {fecha}");
}
void mostrarMovimientosPorCuenta()
{
    string codigo = PedirCodigocuenta();
    if (codigo == "") return;

    List<string[]> todos = LeerTodosLosMovimientos();
    List<string[]> filtrados = new List<string[]>();

    foreach (string[] m in todos)
    {
        if (m.Length == 5 && m[1].StartsWith(codigo))
        {
            filtrados.Add(m);
        }
    }

    MostrarTablaMovimientos(filtrados, $"MOVIMIENTOS DE LA CUENTA {codigo}");
}

// ================= LIBRO MAYOR =================

string ExtraerCodigo(string cuentaJunta)
{
    int posicion = cuentaJunta.IndexOf(" - ");

    if (posicion == -1)
        return cuentaJunta.Trim();

    return cuentaJunta.Substring(0, posicion).Trim();
}

void LimpiarMayor()
{
    mayorCodigo.Clear();
    mayorNombre.Clear();
    mayorTipo.Clear();
    mayorNaturaleza.Clear();
    mayorDebe.Clear();
    mayorHaber.Clear();
    mayorSaldo.Clear();
}

int CargarCatalogoEnMayor()
{
    if (!File.Exists(archivoCatalogo)) return 0;

    using (StreamReader lector = new StreamReader(archivoCatalogo))
    {
        lector.ReadLine(); // encabezado

        string? linea;
        while ((linea = lector.ReadLine()) != null)
        {
            string[] datos = linea.Split(';');

            if (datos.Length == 4)
            {
                mayorCodigo.Add(datos[0]);
                mayorNombre.Add(datos[1]);
                mayorTipo.Add(datos[2]);
                mayorNaturaleza.Add(datos[3]);
                mayorDebe.Add(0);
                mayorHaber.Add(0);
                mayorSaldo.Add(0);
            }
        }
    }
    return mayorCodigo.Count;
}

int BuscarIndiceCuenta(string codigo)
{
    for (int i = 0; i < mayorCodigo.Count; i++)
    {
        if (mayorCodigo[i] == codigo) return i;
    }
    return -1;
}

// Devuelve "" si el movimiento es válido; si no, el mensaje del error
string ValidarMovimiento(string[] mov, out int indice, out decimal debe, out decimal haber)
{
    indice = -1;
    debe = 0;
    haber = 0;

    if (mov.Length != 5)
        return "formato incorrecto";

    string codigo = ExtraerCodigo(mov[1]);
    indice = BuscarIndiceCuenta(codigo);
    if (indice == -1)
        return $"la cuenta '{codigo}' no existe en el catálogo";

    if (!decimal.TryParse(mov[2], out debe) || !decimal.TryParse(mov[3], out haber))
        return "Debe o Haber no son números";

    if (debe < 0 || haber < 0)
        return "hay montos negativos";

    if ((debe == 0 && haber == 0) || (debe > 0 && haber > 0))
        return "debe tener monto solo en Debe o solo en Haber";

    return "";
}

decimal CalcularSaldo(string naturaleza, decimal debe, decimal haber)
{
    if (naturaleza == "Deudora")
        return debe - haber;

    return haber - debe;
}

void CalcularSaldosFinales()
{
    for (int i = 0; i < mayorCodigo.Count; i++)
    {
        mayorSaldo[i] = CalcularSaldo(mayorNaturaleza[i], mayorDebe[i], mayorHaber[i]);
    }
}

void GuardarCsv(string ruta, List<string> lineas)
{
    if (string.IsNullOrWhiteSpace(ruta) || lineas.Count <= 1)
    {
        Console.WriteLine("No hay datos para guardar.");
        return;
    }

    try
    {
        using (StreamWriter escritor = new StreamWriter(ruta, false))
        {
            for (int i = 0; i < lineas.Count; i++)
            {
                escritor.WriteLine(lineas[i]);
            }
        }
        Console.WriteLine($"Archivo guardado: {Path.GetFileName(ruta)}");
    }
    catch (IOException)
    {
        Console.WriteLine($"No se pudo guardar {Path.GetFileName(ruta)}. Si está abierto en Excel, ciérralo.");
    }
}

void GuardarLibroMayorCsv()
{
    List<string> lineas = new List<string>();
    lineas.Add("Codigo;Cuenta;Tipo;Naturaleza;Debe;Haber;Saldo");

    for (int i = 0; i < mayorCodigo.Count; i++)
    {
        lineas.Add($"{mayorCodigo[i]};{mayorNombre[i]};{mayorTipo[i]};{mayorNaturaleza[i]};{mayorDebe[i]:0.00};{mayorHaber[i]:0.00};{mayorSaldo[i]:0.00}");
    }

    GuardarCsv(archivoLibroMayor, lineas);
}

// Calcula el libro mayor y lo guarda en libro_mayor.csv.

bool CalcularLibroMayor()
{
    LimpiarMayor();

    if (CargarCatalogoEnMayor() == 0)
    {
        Console.WriteLine("\nNo hay cuentas en el catálogo.");
        return false;
    }

    List<string[]> movimientos = LeerTodosLosMovimientos();
    if (movimientos.Count == 0)
    {
        Console.WriteLine("\nNo hay movimientos registrados.");
        return false;
    }

    decimal totalDebe = 0;
    decimal totalHaber = 0;

    for (int i = 0; i < movimientos.Count; i++)
    {
        string error = ValidarMovimiento(movimientos[i], out int indice, out decimal debe, out decimal haber);
        if (error != "")
        {
            Console.WriteLine($"\nMovimiento #{i + 1} inválido: {error}.");
            return false;
        }

        mayorDebe[indice] += debe;
        mayorHaber[indice] += haber;
        totalDebe += debe;
        totalHaber += haber;
    }

    if (totalDebe != totalHaber)
    {
        Console.WriteLine($"\nNo se pueden generar los estados: Debe (C$ {totalDebe:N2}) y Haber (C$ {totalHaber:N2}) no cuadran.");
        return false;
    }

    CalcularSaldosFinales();
    GuardarLibroMayorCsv();
    return true;
}

// ================= CÁLCULOS PARA LOS ESTADOS =================

decimal TotalPorTipo(string tipo)
{
    decimal total = 0;

    for (int i = 0; i < mayorTipo.Count; i++)
    {
        if (mayorTipo[i] == tipo) total += mayorSaldo[i];
    }
    return total;
}

decimal CalcularUtilidadOperacion()
{
    return TotalPorTipo("Ingreso") - TotalPorTipo("Costo") - TotalPorTipo("Gasto");
}

// El IR solo se calcula si hay utilidad (si hay pérdida es 0)
decimal CalcularIR(decimal utilidadOperacion)
{
    const decimal TASA_IR = 0.30m;

    if (utilidadOperacion <= 0)
        return 0;

    return Math.Round(utilidadOperacion * TASA_IR, 2);
}

// ================= ESTADO DE RESULTADOS =================

void MostrarEstadoResultados
    (decimal ventasNetas, decimal costoVendido, decimal utilidadBruta,
                             decimal gastosOperacion, decimal utilidadOperacion,
                             decimal impuesto, decimal utilidadNeta)
{
    Console.Clear();

    Console.WriteLine("========================================================");
    Console.WriteLine("                   Fashion American                     ");
    Console.WriteLine("                ESTADO DE RESULTADOS                    ");
    Console.WriteLine($"         Del {fechaInicio:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}");
    Console.WriteLine("========================================================");
    Console.WriteLine();

    Console.WriteLine($"VENTAS NETAS.................... C$ {ventasNetas,12:N2}");
    Console.WriteLine();
    Console.WriteLine($"(-) COSTO DE LO VENDIDO......... C$ {costoVendido,12:N2}");
    Console.WriteLine("--------------------------------------------------------");
    Console.WriteLine($"UTILIDAD BRUTA.................. C$ {utilidadBruta,12:N2}");
    Console.WriteLine();
    Console.WriteLine("GASTOS DE OPERACION");
    Console.WriteLine($"   Gastos Registrados........... C$ {gastosOperacion,12:N2}");
    Console.WriteLine("--------------------------------------------------------");
    Console.WriteLine($"UTILIDAD DE OPERACION........... C$ {utilidadOperacion,12:N2}");
    Console.WriteLine();
    Console.WriteLine($"(-) IR (30%).................... C$ {impuesto,12:N2}");
    Console.WriteLine("--------------------------------------------------------");
    Console.WriteLine($"UTILIDAD NETA DEL EJERCICIO..... C$ {utilidadNeta,12:N2}");
    Console.WriteLine();
    Console.WriteLine("========================================================");
}

void GuardarEstadoResultadosCsv(decimal ventasNetas, decimal costoVendido, decimal utilidadBruta,
                                decimal gastosOperacion, decimal utilidadOperacion,
                                decimal impuesto, decimal utilidadNeta)
{
    List<string> lineas = new List<string>();
    lineas.Add("VentasNetas;CostoVendido;UtilidadBruta;GastosOperacion;UtilidadOperacion;IR;UtilidadNeta");
    lineas.Add($"{ventasNetas:0.00};{costoVendido:0.00};{utilidadBruta:0.00};{gastosOperacion:0.00};{utilidadOperacion:0.00};{impuesto:0.00};{utilidadNeta:0.00}");

    GuardarCsv(archivoEstadoResultados, lineas);
}

void EstadoResultados()
{
    if (!CalcularLibroMayor()) return;

    decimal ventasNetas = TotalPorTipo("Ingreso");
    decimal costoVendido = TotalPorTipo("Costo");
    decimal gastosOperacion = TotalPorTipo("Gasto");

    decimal utilidadBruta = ventasNetas - costoVendido;
    decimal utilidadOperacion = utilidadBruta - gastosOperacion;
    decimal impuesto = CalcularIR(utilidadOperacion);
    decimal utilidadNeta = utilidadOperacion - impuesto;

    MostrarEstadoResultados(ventasNetas, costoVendido, utilidadBruta, gastosOperacion,
                            utilidadOperacion, impuesto, utilidadNeta);

    GuardarEstadoResultadosCsv(ventasNetas, costoVendido, utilidadBruta, gastosOperacion,
                               utilidadOperacion, impuesto, utilidadNeta);
}

// ================= ESTADO DE SITUACIÓN FINANCIERA (BALANCE GENERAL) =================

// Muestra las cuentas de un tipo (solo las que tienen movimientos) y devuelve su total
decimal MostrarSeccion(string titulo, string tipo, List<string> csv)
{
    Console.WriteLine($"\n{titulo}");
    decimal total = 0;

    for (int i = 0; i < mayorCodigo.Count; i++)
    {
        if (mayorTipo[i] == tipo && (mayorDebe[i] != 0 || mayorHaber[i] != 0))
        {
            Console.WriteLine($"   {mayorNombre[i],-30} C$ {mayorSaldo[i],12:N2}");
            csv.Add($"{titulo};{mayorNombre[i]};{mayorSaldo[i]:0.00}");
            total += mayorSaldo[i];
        }
    }

    string etiqueta = "TOTAL " + titulo;
    Console.WriteLine($"   {etiqueta,-30} C$ {total,12:N2}");
    csv.Add($"{titulo};{etiqueta};{total:0.00}");
    return total;
}

void MostrarLineaBalance(string seccion, string etiqueta, decimal monto, List<string> csv)
{
    Console.WriteLine($"   {etiqueta,-30} C$ {monto,12:N2}");
    csv.Add($"{seccion};{etiqueta};{monto:0.00}");
}

void VerificarEcuacionContable(decimal activo, decimal pasivoMasPatrimonio)
{
    if (activo == pasivoMasPatrimonio)
        Console.WriteLine("\nEl balance cuadra: ACTIVO = PASIVO + PATRIMONIO.");
    else
        Console.WriteLine($"\nATENCION: el balance no cuadra. Diferencia = C$ {activo - pasivoMasPatrimonio:N2}");
}

void EstadoSituacionFinanciera()
{
    if (!CalcularLibroMayor()) return;

    List<string> csv = new List<string>();
    csv.Add("Seccion;Cuenta;Monto");

    Console.Clear();
    Console.WriteLine("========================================================");
    Console.WriteLine("                   Fashion American                     ");
    Console.WriteLine("             ESTADO DE SITUACION FINANCIERA             ");
    Console.WriteLine($"                   Al {fechaFin:dd/MM/yyyy}");
    Console.WriteLine("========================================================");

    decimal activo = MostrarSeccion("ACTIVO", "Activo", csv);
    decimal pasivo = MostrarSeccion("PASIVO", "Pasivo", csv);
    decimal patrimonio = MostrarSeccion("PATRIMONIO", "Patrimonio", csv);

    decimal utilidadOperacion = CalcularUtilidadOperacion();
    decimal impuesto = CalcularIR(utilidadOperacion);
    decimal utilidadNeta = utilidadOperacion - impuesto;

    Console.WriteLine("\nRESULTADO DEL EJERCICIO");
    MostrarLineaBalance("RESULTADO", "IR por pagar", impuesto, csv);
    MostrarLineaBalance("RESULTADO", "Utilidad neta del ejercicio", utilidadNeta, csv);

    decimal pasivoMasPatrimonio = pasivo + patrimonio + impuesto + utilidadNeta;
    Console.WriteLine("\n--------------------------------------------------------");
    MostrarLineaBalance("TOTALES", "TOTAL ACTIVO", activo, csv);
    MostrarLineaBalance("TOTALES", "PASIVO + PATRIMONIO", pasivoMasPatrimonio, csv);
    Console.WriteLine("========================================================");

    VerificarEcuacionContable(activo, pasivoMasPatrimonio);
    GuardarCsv(archivoBalanceGeneral, csv);
}