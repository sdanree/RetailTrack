# Flujo de trabajo con Git para RetailTrack

Este proyecto sigue una estrategia de ramificación basada en un modelo GitFlow simplificado.

## Ramas principales

- `develop`: Rama principal de desarrollo.
- `testing`: Rama para staging y pruebas de integración.
- `production`: Rama exclusiva para código en producción.
- `main`: Rama estable que refleja el estado público del proyecto.

---

## Flujo de trabajo recomendado

### 1. Crear una nueva funcionalidad
```bash
git checkout develop
git pull origin develop
git checkout -b feature/nombre-de-la-feature
# Hacés cambios...
git add .
git commit -m "Feature: ..."
git push origin feature/nombre-de-la-feature
```

### 2. Merge a develop (una vez testeado localmente)
```bash
git checkout develop
git pull origin develop
git merge feature/nombre-de-la-feature
git push origin develop
```

### 3. Sincronizar otras ramas con develop

```bash
# Testing
git checkout testing
git reset --hard develop
git push origin testing --force

# Production
git checkout production
git reset --hard develop
git push origin production --force

# Main (si la usás como rama pública)
git checkout main
git reset --hard develop
git push origin main --force
```
