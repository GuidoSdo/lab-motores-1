# AGENT.md

Convenciones del proyecto para agentes de IA (Claude, Copilot, Cursor, etc.) y para desarrolladores humanos. Aplica a todo el repositorio salvo que una subcarpeta agregue reglas mas especificas.

## Stack y contexto

- Unity 6.3 LTS.
- C# moderno (records, pattern matching y `nullable` permitidos donde Unity los soporte).
- Lenguaje de documentacion: **espanol** (comentarios, summaries y tooltips). El codigo va en ingles como es habitual.

## Runtime Scripts

Estas reglas aplican a scripts ubicados en `Assets/_Project/Scripts/Runtime` y sus subcarpetas.

### Documentacion XML (`/// <summary>`)

- Agregar `/// <summary>` en espanol cuando la responsabilidad del script no sea totalmente obvia o cuando el componente orqueste otros sistemas.
- Mantener los summaries breves y orientados a la responsabilidad principal del componente, no a detalles de implementacion.
- Si un script no expone atributos serializados, documentar la intencion en el `summary` de la clase en lugar de forzar configuraciones ficticias.
- Para helpers triviales, DTOs y structs `[Serializable]` sin logica, el summary es opcional.

### Comentarios en linea

- Agregar comentarios solo cuando expliquen una decision importante, un flujo no obvio o el punto exacto donde ocurre una accion clave.
- Evitar comentarios redundantes que solo repitan lo que ya dice el codigo.
- En metodos de ciclo de vida de Unity (`Awake`, `OnEnable`, `Start`, `OnDestroy`) comentar cuando el **orden** de inicializacion o el acoplamiento con otros sistemas no sea evidente.

### Campos serializados

- Usar `[SerializeField] private` por defecto para referencias y parametros configurables desde Unity. Evitar `public` salvo necesidad real.
- Agregar `[Header("...")]` cuando ayude a agrupar atributos en el Inspector.
- Agregar `[Tooltip("...")]` en atributos serializados cuando el campo no sea evidente por nombre, afecte gameplay, depuracion, capas, rangos o el comportamiento de otros componentes.
- No agregar `Tooltip` a campos no serializados ni crear variables solo para mostrar tooltips.
- Cuando aplique, usar `[Range(min, max)]` para acotar valores numericos y reducir la necesidad de explicar rangos en el tooltip.

### Atributos de Unity 6

- `[RequireComponent]`, `[DisallowMultipleComponent]` y `[DefaultExecutionOrder]` son preferibles a documentar dependencias en texto libre.
- Para `ScriptableObject`, usar `[CreateAssetMenu(...)]` con `menuName` coherente con la carpeta del proyecto.
- `[AddComponentMenu]` opcional, solo cuando ayude a encontrar el componente en el Inspector.
- Async: preferir `Awaitable` de Unity 6 sobre `async void`. Documentar cancellation tokens y condiciones de salida cuando el flujo no sea obvio.

### Consistencia de idioma

- Comentarios, tooltips y summaries van siempre en espanol.
- Los nombres de tipos, miembros y variables van en ingles.

## Validacion automatica

El proyecto valida un subconjunto **determinista** de las convenciones:

- `summary`: toda `class` publica o `sealed class` publica debe tener `/// <summary>` en las 5 lineas previas.
- `tooltip`: todo campo con `[SerializeField]` debe tener `[Tooltip(...)]` en las 3 lineas previas.

Las reglas de criterio subjetivo (por ejemplo "summary solo cuando la responsabilidad no sea obvia") no se validan automaticamente: se confia en el autor del cambio y en la revision de codigo.

### GitHub Actions (obligatorio)

El repositorio corre el workflow `.github/workflows/validate-docs.yml` en cada pull request y push a `main` / `develop`. El validador se ejecuta en **modo estricto** (`-Strict`) y **bloquea el merge** si hay hallazgos en archivos dentro de `Assets/_Project/Scripts/Runtime`.

Esto significa que no hace falta que cada dev configure nada en su maquina. Si un PR no cumple las convenciones minimas, GitHub lo marca como fallido y no se puede mergear hasta arreglarlo.

Para que el check sea obligatorio antes de mergear, activar en GitHub:

1. Settings -> Branches -> Branch protection rules.
2. Regla para `main` (y `develop` si corresponde).
3. Marcar **Require status checks to pass before merging** y seleccionar **Validar documentacion de Runtime**.

### Hook local (opcional)

Para quienes prefieran ver los avisos antes de pushear, existe un hook pre-commit opcional en `.githooks/pre-commit` que corre el validador en **modo advisory** (imprime recomendaciones pero nunca bloquea).

Instalacion local: `pwsh -File .\tools\Install-GitHooks.ps1`

Este paso es opcional. La validacion real ocurre en CI; el hook local solo sirve para no descubrir los avisos recien en el PR.

### Uso manual del validador

```powershell
# Advisory sobre archivos staged (lo mismo que corre el hook local)
pwsh -File .\tools\Validate-RuntimeDocumentation.ps1

# Advisory sobre archivos especificos
pwsh -File .\tools\Validate-RuntimeDocumentation.ps1 -Files Assets/_Project/Scripts/Runtime/Player/PlayerController.cs

# Modo estricto (lo que corre CI): termina con exit 1 si hay hallazgos
pwsh -File .\tools\Validate-RuntimeDocumentation.ps1 -Strict
```

## Notas para agentes de IA

Cuando se te pida crear o modificar un script dentro de `Assets/_Project/Scripts/Runtime`:

1. Incluir `/// <summary>` en espanol en la clase cuando orqueste sistemas o no sea trivial.
2. Todo campo `[SerializeField]` debe llevar `[Tooltip("...")]` con texto en espanol salvo que el nombre sea completamente autoexplicativo.
3. Usar `[SerializeField] private` por defecto, no `public`.
4. No inventar campos serializados solo para cumplir con la regla del tooltip: si el script no los necesita, documentar la intencion en el summary de la clase.
5. Al terminar, mencionar explicitamente si alguna regla quedo sin cumplir y por que, para que el humano pueda decidir.
