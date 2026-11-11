using System;
using System.Collections.Generic;

// Estado de la línea que se está leyendo: cuántas letras de la página ya se escribieron y en qué página va.
// Cada pulsación completa la página si se está escribiendo; si ya está completa, pasa a la siguiente o termina.
public class LineaEnPantalla
{
    private readonly IReadOnlyList<string> paginas;
    private readonly float letrasPorSegundo;
    private int indicePagina;
    private float letrasEscritas;

    public LineaEnPantalla(LineaDialogo linea, float letrasPorSegundo)
    {
        Quien = linea.quien ?? "";
        EsPensamiento = linea.pensamiento;
        paginas = linea.Paginas();
        this.letrasPorSegundo = letrasPorSegundo;
        EmpezarPagina(0);
    }

    public string Quien { get; }
    public bool EsPensamiento { get; }
    public bool Terminada { get; private set; }

    public string Pagina => paginas[indicePagina];
    public int LetrasVisibles => (int)letrasEscritas;
    public bool Escribiendo => LetrasVisibles < Pagina.Length;

    public void Avanzar(float segundos)
    {
        if (Terminada || !Escribiendo)
            return;

        letrasEscritas = Math.Min(letrasEscritas + segundos * letrasPorSegundo, Pagina.Length);
    }

    public void Pulsar()
    {
        if (Terminada)
            return;

        if (Escribiendo)
            CompletarPagina();
        else if (indicePagina < paginas.Count - 1)
            EmpezarPagina(indicePagina + 1);
        else
            Terminada = true;
    }

    private void EmpezarPagina(int indice)
    {
        indicePagina = indice;
        letrasEscritas = 0f;

        // Sin velocidad no hay efecto de escritura: la página aparece completa
        if (letrasPorSegundo <= 0f)
            CompletarPagina();
    }

    private void CompletarPagina()
    {
        letrasEscritas = Pagina.Length;
    }
}
