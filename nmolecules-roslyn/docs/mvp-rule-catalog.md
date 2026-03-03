# nMolecules DDD Rule Catalog

Stand: 2026-03-03

Dieses Dokument definiert den ersten gemeinsamen Regelkatalog fuer die DDD-Pruefungen.
Er ist die fachliche Grundlage fuer:

- Roslyn Analyzer in Visual Studio
- VSIX-Integration
- spaetere Visual-Studio-Code-Integration

## Ziel

Alle IDE-Integrationen sollen dieselben fachlichen Regeln verwenden.
Unterschiedlich darf nur die Darstellung in der IDE sein, nicht die Domaenenlogik der Diagnosen.

## Regelgruppen

### Gruppe A: Strukturregeln

- AggregateRoot muss eine Identity besitzen
- Entity muss eine Identity besitzen
- ValueObject muss unveraenderlich sein
- ValueObject soll `IEquatable<T>` implementieren
- ValueObject soll `sealed` sein, sofern es eine Klasse ist

### Gruppe B: Abhaengigkeitsregeln

- AggregateRoot darf kein Repository verwenden
- AggregateRoot darf keinen Service verwenden
- Entity darf kein Repository verwenden
- Entity darf keinen AggregateRoot verwenden
- Entity darf keinen Service verwenden
- Repository darf keinen Service verwenden
- ValueObject darf keine Entity verwenden
- ValueObject darf keinen Service verwenden
- ValueObject darf kein Repository verwenden
- ValueObject darf keinen AggregateRoot verwenden

### Gruppe C: Layering- und Architekturregeln

- Domain Layer darf Infrastructure nicht referenzieren
- Domain Layer darf UI nicht referenzieren
- Application Layer darf UI nicht referenzieren
- Infrastrukturregeln fuer Module und Bounded Contexts werden spaeter erganzt

### Gruppe D: Rollenregeln fuer neue Attribute

- DomainService darf keine Infrastrukturdetails kapseln
- ApplicationService darf keine Domain-Policy direkt ersetzen
- DomainEventHandler darf nur erlaubte Schichtabhaengigkeiten besitzen

## MVP-Scope

Phase 1 konzentriert sich auf Regeln, die bereits im aktuellen Code oder mit kleinem Ausbau technisch erreichbar sind.

### Bereits im Analyzer-Repo vorhanden

- XMoleculesAggregateRoot0001
- XMoleculesAggregateRoot0002
- XMoleculesAggregateRoot0003
- XMoleculesEntity0001
- XMoleculesEntity0002
- XMoleculesEntity0003
- XMoleculesEntity0004
- XMoleculesRepository0001
- XMoleculesValueObject0001
- XMoleculesValueObject0002
- XMoleculesValueObject0003
- XMoleculesValueObject0004
- XMoleculesValueObject0005
- XMoleculesValueObject1001
- XMoleculesValueObject1002

### MVP-Regeln fuer die erste Ausbauphase

- bestehende Regeln stabilisieren und voll dokumentieren
- Regelmetadaten vereinheitlichen
- Release-Tracking bereinigen
- Architekturregeln fuer Layer vorbereiten
- Regeln fuer `DomainService` und `ApplicationService` spezifizieren

## Regeldefinition je Diagnose

Jede Regel soll kuenftig mindestens folgende Informationen haben:

- Diagnose-ID
- fachlicher Name
- technische Beschreibung
- Severity Default
- positive Beispiele
- negative Beispiele
- Quick-Fix-Moeglichkeit ja oder nein
- Dokumentationslink

## Priorisierte Tasks

### Block 1: Bestehende Regeln absichern

- vorhandene Regel-IDs inventarisieren
- alle Regeltexte auf Konsistenz pruefen
- Tests den Regelbeschreibungen zuordnen
- `AnalyzerReleases.*` mit realer Implementierung abgleichen

### Block 2: Technische Basis verbessern

- gemeinsame Abstraktion fuer Regeldefinitionen bewerten
- Diagnoseerzeugung vereinheitlichen
- gemeinsame Testdatenkonventionen definieren
- `.editorconfig`-Konfiguration vorbereiten

### Block 3: Neue Regeln vorbereiten

- Layer-Regeln fuer `nMolecules.Architecture` spezifizieren
- `DomainService`-Regeln spezifizieren
- `ApplicationService`-Regeln spezifizieren
- Bounded-Context- und Modulregeln priorisieren

## Regeln fuer Visual Studio und VS Code

Die folgende Trennung ist wichtig:

- Fachregel und Diagnose-ID leben im Analyzer-Regelkatalog
- Visual Studio liefert Roslyn-Diagnosen, Quick Fixes und VSIX-UX
- VS Code soll dieselben Regeln und IDs anzeigen, unabhaengig vom technischen Adapter

## Definition of Done fuer neue Regeln

Eine neue Regel ist erst fertig, wenn:

- die Fachregel dokumentiert ist
- eine stabile Diagnose-ID existiert
- positive und negative Testfaelle vorhanden sind
- Severity und Message festgelegt sind
- klar ist, ob ein CodeFix angeboten wird
- die Regel in Visual Studio und spaeter in VS Code dieselbe Bedeutung hat
