# nMolecules Service Role Matrix

Stand: 2026-03-03

Dieses Dokument beschreibt die fachliche Trennung zwischen `Service`, `DomainService` und `ApplicationService`.
Es ist die Grundlage fuer kuenftige dedizierte Analyzer-Regeln.

## Ziel

Aktuell behandelt der Analyzer `DomainService` und `ApplicationService` bewusst wie `Service`, damit die bestehenden Regeln sofort greifen.
Langfristig reicht das nicht aus, weil die drei Rollen fachlich nicht identisch sind.

## Rollen

### `Service`

Bedeutung:

- historischer, allgemeiner Marker im aktuellen API-Modell

Einordnung:

- kurzfristig kompatibler Sammelbegriff
- sollte fuer neue Regeln nicht die einzige semantische Basis bleiben

### `DomainService`

Bedeutung:

- Teil des Domain-Modells
- kapselt Domain-Logik, die nicht natuerlich in Entity, Aggregate oder ValueObject liegt

Erwartungen:

- keine Infrastrukturverantwortung
- keine UI-Verantwortung
- soll Domain-Logik ausdruecken, nicht nur orchestrieren

### `ApplicationService`

Bedeutung:

- orchestriert Use Cases
- koordiniert Domain-Objekte und Infrastrukturzugriffe
- gehoert zur Application-Schicht, nicht zum Domain-Modell

Erwartungen:

- darf Workflows koordinieren
- darf Repositories und Infrastruktur-abstraktionen konsumieren
- soll keine eigentliche Domain-Policy ersetzen

## Kurzfristige Regelstrategie

Phase 1:

- `DomainService` und `ApplicationService` werden in allgemeinen Service-Verboten mitberuecksichtigt
- dadurch bleiben bestehende Regeln konservativ und sicher

Beispiele:

- Entity darf `DomainService` nicht referenzieren
- Entity darf `ApplicationService` nicht referenzieren
- Repository darf `DomainService` nicht referenzieren
- Repository darf `ApplicationService` nicht referenzieren
- ValueObject darf keine der drei Service-Rollen referenzieren

## Zielregeln fuer spaetere Phasen

### Regeln fuer `DomainService`

- DomainService darf nicht im Infrastructure Layer liegen
- DomainService soll keine UI-Typen referenzieren
- DomainService soll keine technischen Framework-Typen als primäre API tragen
- DomainService darf keine ApplicationService-Typen referenzieren

### Regeln fuer `ApplicationService`

- ApplicationService darf Domain-Objekte koordinieren
- ApplicationService darf Repositories konsumieren
- ApplicationService darf DomainService konsumieren
- ApplicationService soll nicht selbst als Domain-Baustein verwendet werden

### Regeln fuer historischen `Service`

- bestehende Kompatibilitaet erhalten
- fuer neue Projekte bevorzugt `DomainService` oder `ApplicationService` statt nur `Service`
- spaeter Pruefung auf praeziseren Marker evaluieren

## Offene Entscheidungen

- Soll `Service` langfristig deprecated werden oder als Alias fuer `DomainService` bestehen bleiben?
- Braucht `ApplicationService` eigene Diagnose-IDs oder reicht eine Layer-Regel?
- Sollen Repositories Application Services wirklich strikt verbieten oder nur Domain Services?

## Naechste technische Schritte

1. Release-Tracking stabilisieren
2. Diagnose-IDs fuer dedizierte Service-Rollen festlegen
3. neue Analyzer fuer `DomainService` und `ApplicationService` entwerfen
4. Layer-Regeln mit den Service-Rollen verknuepfen
