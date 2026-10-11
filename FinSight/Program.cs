//variable necesaria para terminar el ciclo principal del programa
bool continuar = true;

while (continuar)
{
    Console.Clear();

    MostrarTitulo();

    MostrarMenu();

    int opcion = PedirOpcion("Ingrese una opcion: ");

    if (opcion == 9)
    {
        continuar = false;
    }

    switch (opcion)
    {
        case 1:
            break;
        case 2:
            break;
        case 3:
            break;
        case 4:
            break;
        case 5:
            break;
        case 6:
            break;
        case 7:
            break;
        case 8:
            break;
        case 9:
            break;
        default:
            Console.WriteLine("Opcion fuera de rango. Ingrese una opcion del 1 al 9.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }

}

void MostrarTitulo()
{
    Console.WriteLine("========================================");
    Console.WriteLine("            SISTEMA CONTABLE            ");
    Console.WriteLine("========================================");
}
void MostrarMenu()
{
    Console.WriteLine("1. Administrar catálogo de cuentas");
    Console.WriteLine("2. Registrar movimiento contable");
    Console.WriteLine("3. Consultar catálogo de cuentas");
    Console.WriteLine("4. Consultar movimientos ");
    Console.WriteLine("5. Estado de Resultados");
    Console.WriteLine("6. Estado de Situación Financiera");
    Console.WriteLine("7. Estado de Cambios en el Patrimonio");
    Console.WriteLine("8. Estado de Flujos de Efectivo");
    Console.WriteLine("9. Salir");
}
int PedirOpcion(string mensaje)
{
    while (true)
    {
        Console.Write(mensaje);
        string entrada = Console.ReadLine() ?? "";

        if (int.TryParse(entrada, out int opcion))
        {
            return opcion;
        }

        Console.WriteLine("Opcion invalida. Debe ingresar un numero entero.");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}