using System.Reflection;
using System;
using System.Reflection;
using UnityEngine;
public class TestTablero : MonoBehaviour
{
    private int correctos;
    private int fallidos;

    private GameObject raizPrueba;
    private TableroUI tablero;
    private Mazo mazo;
    private Transform mano;
    private SlotPosicion[] slots;

    private void Start()
    {
        Debug.Log("=== INICIO TEST_TABLERO ===");

        PrepararTablero();

        TestTableroInicialmenteVacio();
        TestNoTodasColocadasConUnSoloSlot();
        TestTodasColocadas();
        TestObtenerJugadasEnOrden();
        TestLiberarSlot();
        TestBloquearCartas();
        TestDesbloquearCartas();
        TestDevolverCartaAMano();
        TestNoDevolverCartaBloqueada();
        TestNoDevolverCartaQueYaEstaEnLaMano();
        TestSeleccionarSoloCartaDeLaMano();
        TestDeseleccionarCarta();
        TestSeleccionarCartaNula();

        MostrarResultado();
        Limpiar();
    }

    private void PrepararTablero()
    {
        raizPrueba = new GameObject("Tablero_Prueba");

        // Crear la mano.
        GameObject objetoMano =
            new GameObject(
                "Mano_Prueba",
                typeof(RectTransform)
            );

        objetoMano.transform.SetParent(
            raizPrueba.transform,
            false
        );

        mano = objetoMano.transform;

        // Crear Mazo, pero desactivado para impedir que se ejecute su Start.
        GameObject objetoMazo =
            new GameObject("Mazo_Prueba");

        objetoMazo.transform.SetParent(
            raizPrueba.transform,
            false
        );

        mazo = objetoMazo.AddComponent<Mazo>();
        mazo.enabled = false;
        mazo.mano = mano;
        mazo.tamMano = 5;

        // Crear los tres slots.
        slots = new SlotPosicion[3];

        slots[0] = CrearSlot(
            "Slot_Pasado",
            TipoPosicion.Pasado
        );

        slots[1] = CrearSlot(
            "Slot_Presente",
            TipoPosicion.Presente
        );

        slots[2] = CrearSlot(
            "Slot_Futuro",
            TipoPosicion.Futuro
        );

        // Crear TableroUI.
        GameObject objetoTablero =
            new GameObject("TableroUI_Prueba");

        objetoTablero.transform.SetParent(
            raizPrueba.transform,
            false
        );

        tablero =
            objetoTablero.AddComponent<TableroUI>();

        AsignarCampoPrivado(
            tablero,
            "mazo",
            mazo
        );

        AsignarCampoPrivado(
            tablero,
            "slots",
            slots
        );
    }

    private SlotPosicion CrearSlot(
        string nombre,
        TipoPosicion tipo)
    {
        GameObject objeto =
            new GameObject(
                nombre,
                typeof(RectTransform)
            );

        objeto.transform.SetParent(
            raizPrueba.transform,
            false
        );

        SlotPosicion slot =
            objeto.AddComponent<SlotPosicion>();

        AsignarCampoPrivado(
            slot,
            "tipo",
            tipo
        );

        return slot;
    }

    private CartaArrastrable CrearCarta(
        string nombre,
        Carta carta,
        Transform padre)
    {
        GameObject objeto =
            new GameObject(
                nombre,
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(CartaVisual),
                typeof(CartaArrastrable)
            );

        objeto.transform.SetParent(
            padre,
            false
        );

        CartaVisual visual =
            objeto.GetComponent<CartaVisual>();

        AsignarPropiedadAutomatica(
            visual,
            "Carta",
            carta
        );

        CartaArrastrable arrastrable =
            objeto.GetComponent<CartaArrastrable>();

        arrastrable.PuedeArrastrarse = true;

        return arrastrable;
    }

    private void VaciarTablero()
    {
        foreach (SlotPosicion slot in slots)
        {
            if (slot.Ocupado)
            {
                CartaVisual ocupante =
                    slot.Ocupante;

                slot.Liberar();

                if (ocupante != null)
                {
                    Destroy(ocupante.gameObject);
                }
            }
        }

        for (int i = mano.childCount - 1; i >= 0; i--)
        {
            Destroy(mano.GetChild(i).gameObject);
        }
    }

    private void ColocarTresCartas()
    {
        CartaArrastrable pasado =
            CrearCarta(
                "Carta_Pasado",
                new Carta(Palo.Llama, 3),
                mano
            );

        CartaArrastrable presente =
            CrearCarta(
                "Carta_Presente",
                new Carta(Palo.Velo, 7),
                mano
            );

        CartaArrastrable futuro =
            CrearCarta(
                "Carta_Futuro",
                new Carta(Palo.Raiz, 12),
                mano
            );

        slots[0].Colocar(pasado);
        slots[1].Colocar(presente);
        slots[2].Colocar(futuro);
    }

    private void TestTableroInicialmenteVacio()
    {
        VaciarTablero();

        bool todosVacios =
            !slots[0].Ocupado &&
            !slots[1].Ocupado &&
            !slots[2].Ocupado;

        Comprobar(
            todosVacios &&
            !tablero.TodasColocadas(),
            "Un tablero vacío no tiene todas las cartas colocadas"
        );
    }

    private void TestNoTodasColocadasConUnSoloSlot()
    {
        VaciarTablero();

        CartaArrastrable carta =
            CrearCarta(
                "Carta_Unica",
                new Carta(Palo.Llama, 5),
                mano
            );

        slots[0].Colocar(carta);

        Comprobar(
            slots[0].Ocupado &&
            !slots[1].Ocupado &&
            !slots[2].Ocupado &&
            !tablero.TodasColocadas(),
            "Una sola carta no completa el tablero"
        );
    }

    private void TestTodasColocadas()
    {
        VaciarTablero();
        ColocarTresCartas();

        Comprobar(
            tablero.TodasColocadas(),
            "El tablero está completo cuando los tres slots están ocupados"
        );
    }

    private void TestObtenerJugadasEnOrden()
    {
        VaciarTablero();
        ColocarTresCartas();

        Carta[] jugadas =
            tablero.ObtenerJugadas();

        bool resultadoCorrecto =
            jugadas != null &&
            jugadas.Length == 3 &&
            jugadas[0] != null &&
            jugadas[1] != null &&
            jugadas[2] != null &&
            jugadas[0].Palo == Palo.Llama &&
            jugadas[0].Valor == 3 &&
            jugadas[1].Palo == Palo.Velo &&
            jugadas[1].Valor == 7 &&
            jugadas[2].Palo == Palo.Raiz &&
            jugadas[2].Valor == 12;

        Comprobar(
            resultadoCorrecto,
            "ObtenerJugadas respeta el orden Pasado, Presente y Futuro"
        );
    }

    private void TestLiberarSlot()
    {
        VaciarTablero();

        CartaArrastrable carta =
            CrearCarta(
                "Carta_Liberar",
                new Carta(Palo.Abismo, 9),
                mano
            );

        slots[0].Colocar(carta);
        slots[0].Liberar();

        Comprobar(
            !slots[0].Ocupado &&
            slots[0].Ocupante == null,
            "Liberar deja el slot vacío"
        );
    }

    private void TestBloquearCartas()
    {
        VaciarTablero();
        ColocarTresCartas();

        tablero.BloquearSlots(true);

        bool bloqueadas = true;

        foreach (SlotPosicion slot in slots)
        {
            CartaArrastrable carta =
                slot.Ocupante.GetComponent<CartaArrastrable>();

            if (carta.PuedeArrastrarse)
            {
                bloqueadas = false;
            }
        }

        Comprobar(
            bloqueadas,
            "BloquearSlots impide arrastrar las cartas colocadas"
        );
    }

    private void TestDesbloquearCartas()
    {
        VaciarTablero();
        ColocarTresCartas();

        tablero.BloquearSlots(true);
        tablero.BloquearSlots(false);

        bool desbloqueadas = true;

        foreach (SlotPosicion slot in slots)
        {
            CartaArrastrable carta =
                slot.Ocupante.GetComponent<CartaArrastrable>();

            if (!carta.PuedeArrastrarse)
            {
                desbloqueadas = false;
            }
        }

        Comprobar(
            desbloqueadas,
            "BloquearSlots(false) permite volver a arrastrar las cartas"
        );
    }

    private void TestDevolverCartaAMano()
    {
        VaciarTablero();

        CartaArrastrable carta =
            CrearCarta(
                "Carta_Devolver",
                new Carta(Palo.Velo, 6),
                mano
            );

        slots[1].Colocar(carta);

        tablero.DevolverCartaAMano(carta);

        Comprobar(
            !slots[1].Ocupado &&
            carta.transform.parent == mano,
            "DevolverCartaAMano libera el slot y devuelve la carta"
        );
    }

    private void TestNoDevolverCartaBloqueada()
    {
        VaciarTablero();

        CartaArrastrable carta =
            CrearCarta(
                "Carta_Bloqueada",
                new Carta(Palo.Raiz, 8),
                mano
            );

        slots[2].Colocar(carta);
        carta.PuedeArrastrarse = false;

        tablero.DevolverCartaAMano(carta);

        Comprobar(
            slots[2].Ocupado &&
            carta.transform.parent == slots[2].transform,
            "Una carta bloqueada no puede volver a la mano"
        );
    }

    private void TestNoDevolverCartaQueYaEstaEnLaMano()
    {
        VaciarTablero();

        CartaArrastrable carta =
            CrearCarta(
                "Carta_En_Mano",
                new Carta(Palo.Llama, 10),
                mano
            );

        tablero.DevolverCartaAMano(carta);

        Comprobar(
            carta.transform.parent == mano &&
            !slots[0].Ocupado &&
            !slots[1].Ocupado &&
            !slots[2].Ocupado,
            "Devolver una carta que ya está en la mano no modifica el tablero"
        );
    }

    private void TestSeleccionarSoloCartaDeLaMano()
    {
        VaciarTablero();

        CartaArrastrable cartaMano =
            CrearCarta(
                "Carta_Seleccionable",
                new Carta(Palo.Abismo, 4),
                mano
            );

        CartaVisual visual =
            cartaMano.GetComponent<CartaVisual>();

        tablero.Seleccionar(visual);

        CartaVisual seleccionada =
            ObtenerCampoPrivado<CartaVisual>(
                tablero,
                "seleccionada"
            );

        Comprobar(
            seleccionada == visual,
            "Se puede seleccionar una carta situada en la mano"
        );
    }

    private void TestDeseleccionarCarta()
    {
        VaciarTablero();

        CartaArrastrable carta =
            CrearCarta(
                "Carta_Deseleccionar",
                new Carta(Palo.Llama, 11),
                mano
            );

        tablero.Seleccionar(
            carta.GetComponent<CartaVisual>()
        );

        tablero.Deseleccionar();

        CartaVisual seleccionada =
            ObtenerCampoPrivado<CartaVisual>(
                tablero,
                "seleccionada"
            );

        Comprobar(
            seleccionada == null,
            "Deseleccionar elimina la carta seleccionada"
        );
    }

    private void TestSeleccionarCartaNula()
    {
        tablero.Seleccionar(null);

        CartaVisual seleccionada =
            ObtenerCampoPrivado<CartaVisual>(
                tablero,
                "seleccionada"
            );

        Comprobar(
            seleccionada == null,
            "Seleccionar null no produce una selección"
        );
    }

    private void AsignarCampoPrivado(
        object objeto,
        string nombreCampo,
        object valor)
    {
        FieldInfo campo =
            objeto.GetType().GetField(
                nombreCampo,
                BindingFlags.Instance |
                BindingFlags.NonPublic
            );

        if (campo == null)
        {
            throw new MissingFieldException(
                objeto.GetType().Name,
                nombreCampo
            );
        }

        campo.SetValue(objeto, valor);
    }

    private T ObtenerCampoPrivado<T>(
        object objeto,
        string nombreCampo)
    {
        FieldInfo campo =
            objeto.GetType().GetField(
                nombreCampo,
                BindingFlags.Instance |
                BindingFlags.NonPublic
            );

        if (campo == null)
        {
            throw new MissingFieldException(
                objeto.GetType().Name,
                nombreCampo
            );
        }

        return (T)campo.GetValue(objeto);
    }

    private void AsignarPropiedadAutomatica(
        object objeto,
        string nombrePropiedad,
        object valor)
    {
        string nombreCampo =
            "<" + nombrePropiedad + ">k__BackingField";

        FieldInfo campo =
            objeto.GetType().GetField(
                nombreCampo,
                BindingFlags.Instance |
                BindingFlags.NonPublic
            );

        if (campo == null)
        {
            throw new MissingFieldException(
                objeto.GetType().Name,
                nombreCampo
            );
        }

        campo.SetValue(objeto, valor);
    }

    private void Comprobar(
        bool condicion,
        string nombre)
    {
        if (condicion)
        {
            correctos++;
            Debug.Log("[OK] " + nombre);
        }
        else
        {
            fallidos++;
            Debug.LogError("[FALLO] " + nombre);
        }
    }

    private void MostrarResultado()
    {
        string resultado =
            "=== RESULTADO TEST_TABLERO: " +
            correctos + " correctos, " +
            fallidos + " fallidos ===";

        if (fallidos == 0)
        {
            Debug.Log(resultado);
        }
        else
        {
            Debug.LogError(resultado);
        }
    }

    private void Limpiar()
    {
        if (raizPrueba != null)
        {
            Destroy(raizPrueba);
        }
    }
}