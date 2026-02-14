# 🎮 SCRIPT DE BOTÓN PARA CAMBIAR ESCENAS

## 📋 CARACTERÍSTICAS

✅ Arrastra y suelta en cualquier botón UI
✅ Configura el nombre de la escena en el Inspector
✅ Funciona automáticamente
✅ Delay opcional antes de cambiar
✅ Debug para verificar que funciona
✅ Manejo de errores completo

---

## 🚀 CONFIGURACIÓN RÁPIDA (3 PASOS)

### PASO 1: Crear un botón UI

Si aún no tienes un botón:

1. Click derecho en la jerarquía
2. **UI → Button - TextMeshPro** (o Button si usas el UI antiguo)
3. Unity creará automáticamente un Canvas y el botón

---

### PASO 2: Añadir el script al botón

1. Selecciona el botón en la jerarquía
2. En el Inspector, click en **Add Component**
3. Busca y añade: **BotonCambiarEscena**
4. El script se añadirá automáticamente

---

### PASO 3: Configurar el nombre de la escena

En el Inspector del botón:

```
BOTON CAMBIAR ESCENA (SCRIPT):

CONFIGURACIÓN DE ESCENA:
├─ Nombre Escena: "NombreDeTuEscena" ← Escribe aquí el nombre

OPCIONES ADICIONALES (OPCIONAL):
├─ Mostrar Debug: ✗ (desactivado)
└─ Delay Antes De Cambiar: 0
```

**Ejemplo:**
```
Nombre Escena: "MenuPrincipal"
Nombre Escena: "Nivel1"
Nombre Escena: "GameOver"
```

---

## ⚠️ IMPORTANTE: AÑADIR ESCENAS AL BUILD

Para que funcione, **la escena debe estar en Build Settings**:

### Cómo añadir escenas:

1. Ve a **File → Build Settings**
2. Arrastra tus escenas desde el Project a **"Scenes In Build"**
3. O haz click en **"Add Open Scenes"** si la escena está abierta

**Debe verse así:**
```
Scenes In Build:
✅ 0: MenuPrincipal
✅ 1: Nivel1
✅ 2: Nivel2
✅ 3: GameOver
```

Si no están añadidas, el juego dará error al intentar cambiar de escena.

---

## 📝 EJEMPLOS DE USO

### Ejemplo 1: Botón "Jugar" en el Menú Principal

**Configuración:**
```
Nombre Escena: "Nivel1"
Delay Antes De Cambiar: 0
```

---

### Ejemplo 2: Botón "Volver al Menú"

**Configuración:**
```
Nombre Escena: "MenuPrincipal"
Delay Antes De Cambiar: 0
```

---

### Ejemplo 3: Botón "Reintentar" en Game Over

**Configuración:**
```
Nombre Escena: "Nivel1"
Delay Antes De Cambiar: 0.5
```

---

### Ejemplo 4: Botón "Salir del Juego"

Para salir del juego necesitas un script diferente. Usa este:

```csharp
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotonSalir : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Salir);
    }

    void Salir()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
```

---

## 🎨 CONFIGURACIÓN COMPLETA DEL BOTÓN

### En el Inspector verás:

```
┌────────────────────────────────────────────┐
│ BUTTON (SCRIPT)                            │
│ ├─ Interactable: ✓                        │
│ └─ Navigation: Automatic                   │
├────────────────────────────────────────────┤
│ BOTON CAMBIAR ESCENA (SCRIPT)              │
│                                            │
│ CONFIGURACIÓN DE ESCENA:                   │
│ └─ Nombre Escena: [Escribe aquí]          │
│                                            │
│ OPCIONES ADICIONALES:                      │
│ ├─ Mostrar Debug: ☐                       │
│ └─ Delay Antes De Cambiar: 0              │
└────────────────────────────────────────────┘
```

---

## 🔧 OPCIONES ADICIONALES

### Mostrar Debug

```
Mostrar Debug: ✓ (activado)
```

**¿Qué hace?**
- Muestra mensajes en la consola cuando:
  - El botón se configura al inicio
  - Haces click en el botón
  - La escena se está cargando

**Útil para:**
- Verificar que el script está funcionando
- Debugging si algo no funciona
- Ver exactamente cuándo se cambia de escena

---

### Delay Antes De Cambiar

```
Delay Antes De Cambiar: 0.5
```

**¿Qué hace?**
- Espera X segundos antes de cambiar de escena
- Útil para reproducir sonidos o animaciones

**Valores recomendados:**
- `0` = Cambio instantáneo (recomendado normalmente)
- `0.3-0.5` = Permite escuchar un sonido de click
- `1-2` = Para transiciones con fade out

---

## 🎯 EJEMPLO COMPLETO: MENÚ PRINCIPAL

### Estructura de UI recomendada:

```
Canvas
├─ PanelMenu
│   ├─ TituloJuego (Text)
│   ├─ BotonJugar (Button)
│   │   └─ Script: BotonCambiarEscena → "Nivel1"
│   ├─ BotonOpciones (Button)
│   │   └─ Script: BotonCambiarEscena → "Opciones"
│   ├─ BotonCreditos (Button)
│   │   └─ Script: BotonCambiarEscena → "Creditos"
│   └─ BotonSalir (Button)
│       └─ Script: BotonSalir
```

---

## 🐛 SOLUCIÓN DE PROBLEMAS

### Error: "Scene 'NombreEscena' couldn't be loaded"

**Causa:** La escena no está en Build Settings

**Solución:**
1. File → Build Settings
2. Arrastra la escena a "Scenes In Build"

---

### Error: No pasa nada al hacer click

**Posibles causas:**

1. **Nombre de escena vacío**
   - Verifica que escribiste el nombre en el Inspector

2. **Nombre de escena incorrecto**
   - El nombre debe ser EXACTO (case-sensitive)
   - "Nivel1" ≠ "nivel1" ≠ "Nivel 1"

3. **EventSystem falta**
   - Cuando creas UI, Unity crea un EventSystem
   - Si lo borraste, créalo: GameObject → UI → Event System

4. **Botón no interactuable**
   - Verifica que "Interactable" esté marcado en el Button

---

### El botón funciona en el Editor pero no en Build

**Solución:**
- Asegúrate de que la escena esté en Build Settings
- Haz un nuevo Build después de añadir las escenas

---

## 💡 TIPS Y MEJORES PRÁCTICAS

### 1. Nombres de Escenas Consistentes

Usa nombres claros y consistentes:
```
✅ BIEN:
   - MenuPrincipal
   - Nivel1
   - Nivel2
   - GameOver

❌ MAL:
   - menu principal
   - level_1
   - lvl2
   - game over screen
```

---

### 2. Organiza tus Escenas

Estructura recomendada en el Project:
```
Assets/
├─ Scenes/
│   ├─ Menu/
│   │   ├─ MenuPrincipal.unity
│   │   ├─ Opciones.unity
│   │   └─ Creditos.unity
│   ├─ Niveles/
│   │   ├─ Nivel1.unity
│   │   ├─ Nivel2.unity
│   │   └─ Nivel3.unity
│   └─ UI/
│       └─ GameOver.unity
```

---

### 3. Activa Debug Temporalmente

Durante desarrollo:
```
Mostrar Debug: ✓
```

Antes de publicar:
```
Mostrar Debug: ✗
```

---

### 4. Usa Delay para Transiciones

Si tienes un fade out o sonido:
```
Delay Antes De Cambiar: 0.5
```

Esto permite que termine la animación/sonido antes de cambiar.

---

## 🎓 CÓDIGO EXTRA: BOTONES COMUNES

### Botón Reiniciar Nivel Actual

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class BotonReiniciar : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Reiniciar);
    }

    void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
```

---

### Botón Cargar Siguiente Nivel

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class BotonSiguienteNivel : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SiguienteNivel);
    }

    void SiguienteNivel()
    {
        int nivelActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(nivelActual + 1);
    }
}
```

---

### Botón Cargar Nivel Anterior

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class BotonNivelAnterior : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(NivelAnterior);
    }

    void NivelAnterior()
    {
        int nivelActual = SceneManager.GetActiveScene().buildIndex;
        if (nivelActual > 0)
        {
            SceneManager.LoadScene(nivelActual - 1);
        }
    }
}
```

---

## 📝 CHECKLIST DE CONFIGURACIÓN

- [ ] Script añadido al botón
- [ ] Nombre de escena configurado en el Inspector
- [ ] Escena añadida a Build Settings (File → Build Settings)
- [ ] Nombre de escena escrito correctamente (case-sensitive)
- [ ] EventSystem presente en la escena
- [ ] Botón "Interactable" está activado
- [ ] Probé el botón en Play mode
- [ ] Funciona correctamente

---

## 🎉 ¡LISTO!

Ahora puedes:
✅ Crear botones de navegación fácilmente
✅ Reutilizar el script en múltiples botones
✅ Configurar cada botón con una escena diferente
✅ Tener menús completos funcionando

**¡Solo arrastra, configura el nombre, y funciona!** 🚀
