# lab-motores-1

Proyecto Unity para **Clases de Motores 1 - 2026 C1**.

## Version del proyecto

- Unity Editor: `6000.3.12f1`
- Rama principal: `main`
- Control de versiones: Git + GitHub

Abrir el proyecto desde la raiz del repositorio, no desde `Assets`.

## Estructura del repositorio

Unity usa carpetas de proyecto con responsabilidades distintas:

```text
lab-motores-1/
  Assets/              Assets importados por Unity y contenido del juego.
  Packages/            Manifest y lock de paquetes del Package Manager.
  ProjectSettings/     Configuracion versionable del proyecto.
  .github/             Templates y documentacion de GitHub.
  .gitignore           Archivos generados que no se suben.
  .gitattributes       Reglas de texto/binario para Git.
```

No se versionan `Library/`, `Temp/`, `Obj/`, `Logs/`, `Build/`, `Builds/` ni archivos generados por IDE.

## Estructura de Assets

La convencion del proyecto es separar el contenido propio dentro de `Assets/_Project` para no mezclarlo con paquetes, samples o carpetas generadas por Unity.

```text
Assets/
  _Project/
    Animations/
    Art/
      Materials/
      Models/
      Sprites/
    Audio/
    Editor/
    Input/
    Prefabs/
    Scenes/
    Scripts/
      Runtime/
      Editor/
    Settings/
    Shaders/
    Tests/
      EditMode/
      PlayMode/
  Scenes/
  Settings/
  TutorialInfo/
```

Uso esperado:

- `Assets/_Project/Scenes`: escenas propias nuevas del proyecto.
- `Assets/Scenes`: escenas existentes o creadas por el template inicial.
- `Assets/_Project/Scripts/Runtime`: codigo que entra al build del juego.
- `Assets/_Project/Scripts/Editor`: herramientas de editor y scripts que no entran al build.
- `Assets/_Project/Editor`: assets o utilidades globales exclusivas del editor.
- `Assets/_Project/Tests/EditMode`: tests de editor.
- `Assets/_Project/Tests/PlayMode`: tests que requieren modo Play.

Evitar crear carpetas de juego en la raiz del repositorio, por ejemplo `Scenes/` fuera de `Assets`. Unity solo importa como assets del proyecto lo que vive dentro de `Assets`.

## Reglas de trabajo

- Hacer cambios en una rama distinta de `main`.
- Un PR debe representar una unidad chica y revisable: una feature, bugfix, ajuste de escena o refactor puntual.
- Antes de mergear, revisar el template de PR completo.
- No subir archivos generados por Unity o IDE.
- No borrar ni regenerar `.meta` sin entender el impacto.
- Mover o renombrar assets desde Unity cuando sea posible, para conservar referencias.
- Si se mueve un asset fuera de Unity, mover tambien su `.meta` correspondiente.
- Evitar editar la misma escena o prefab en paralelo entre varias ramas.
- Para escenas y prefabs con conflictos, preferir resolver con UnityYAMLMerge.
- Mantener nombres claros y consistentes: `PascalCase` para scripts C#, `kebab-case` o `PascalCase` para assets segun el tipo, pero sin mezclar estilos dentro de la misma carpeta.
- No dejar objetos temporales, pruebas sueltas o comentarios de debugging en escenas compartidas.
- Documentar decisiones importantes en el PR.

## Buenas practicas Unity

- Activar `Visible Meta Files` en Unity cuando se trabaja con Git.
- Usar serializacion de assets en texto para poder revisar escenas, prefabs y assets YAML en Git.
- Versionar juntos el asset y su archivo `.meta`.
- Usar `Editor` solo para codigo/herramientas que no deben entrar al build.
- Usar `Resources` solamente cuando haya una razon clara; carga todo de forma especial y puede crecer sin control.
- Usar `StreamingAssets` solo para archivos que deban copiarse al build sin ser importados normalmente.
- Considerar Assembly Definitions (`.asmdef`) cuando el proyecto crezca y haya modulos claros.

## Flujo recomendado sin GitHub Team

Como el repo no usa branch protection paga ni rulesets avanzados, el control se hace por proceso:

1. Crear rama desde `main`: `feature/nombre-corto`, `fix/nombre-corto` o `docs/nombre-corto`.
2. Hacer commits chicos con mensajes claros.
3. Abrir PR hacia `main`.
4. Completar el checklist del template.
5. Revisar archivos cambiados en GitHub antes de mergear.
6. Probar que Unity abra sin errores.
7. Hacer merge solo cuando el PR este entendible y verificable.
8. Borrar la rama despues del merge.

## Validacion local antes de PR

Checklist minimo:

- Unity abre el proyecto sin errores nuevos en Console.
- Las escenas modificadas abren correctamente.
- No hay referencias rotas visibles en prefabs o escenas.
- `git status` no muestra archivos generados innecesarios.
- El PR no incluye `Library/`, `Temp/`, `Obj/`, `Logs/` ni builds.
- Los cambios grandes de assets estan justificados en la descripcion.

## Referencias oficiales

- Unity 6.3 LTS Manual - Project configuration: https://docs.unity3d.com/6000.3/Documentation/Manual/project-configuration.html
- Unity 6.3 LTS Manual - Version Control: https://docs.unity3d.com/6000.3/Documentation/Manual/class-VersionControlSettings.html
- Unity 6.3 LTS Manual - Asset metadata: https://docs.unity3d.com/6000.3/Documentation/Manual/AssetMetadata.html
- Unity 6.3 LTS Manual - Reserved folder names: https://docs.unity3d.com/6000.3/Documentation/Manual/SpecialFolders.html
- Unity 6.3 LTS Manual - Smart merge: https://docs.unity3d.com/6000.3/Documentation/Manual/SmartMerge.html
- Unity 6.3 LTS Manual - Assemblies: https://docs.unity3d.com/6000.3/Documentation/Manual/assembly-definitions-intro.html
