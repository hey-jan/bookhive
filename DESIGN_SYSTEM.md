# BookHive Design System

This app uses a custom bookstore/editorial design language layered on top of Bootstrap.

## Design Direction

- Warm, paper-like backgrounds
- Editorial typography with a serif display face and sans-serif body text
- Soft, premium surfaces with subtle borders and shadows
- Earth-toned accents inspired by books, leather, and print materials

## Color Tokens

Defined in [BookHive/wwwroot/css/site.css](./BookHive/wwwroot/css/site.css).

| Token | Value | Usage |
| --- | --- | --- |
| `--bh-ink` | `#1e1b16` | Primary text |
| `--bh-muted` | `#685f55` | Secondary text |
| `--bh-paper` | `#f7f1e8` | Base paper tone |
| `--bh-panel` | `rgba(255, 252, 247, 0.9)` | Panels and cards |
| `--bh-border` | `rgba(77, 59, 35, 0.12)` | Borders and dividers |
| `--bh-brand` | `#9f4f2f` | Primary brand accent |
| `--bh-brand-dark` | `#7a3419` | Hover states and stronger accents |
| `--bh-accent` | `#ddb56a` | Highlight accent |

## Typography

Defined in [BookHive/Views/Shared/_Layout.cshtml](./BookHive/Views/Shared/_Layout.cshtml).

- Headings and brand: `Literata`
- Body text: `Manrope`
- Fallback heading stack: `Georgia, serif`
- Fallback body stack: `"Segoe UI", sans-serif`

## Surfaces

- Background uses layered warm gradients to create a paper-like canvas
- Navigation uses a translucent light surface with blur
- Cards and panels use soft ivory fills, thin warm borders, and elevated shadows
- Footer uses a dark, ink-heavy panel with warm link color accents

## Radius and Shape

- Main panels: `1.5rem`
- Cards: `1.25rem`
- Inputs and buttons: about `0.95rem`
- Outline buttons use pill radius

## Shadows

- Primary elevation: `0 24px 60px rgba(38, 27, 16, 0.12)`
- Hover elevation increases depth slightly for book cards

## Component Style Notes

- Primary buttons use a warm brown gradient
- Links use the dark brand brown and lighten on hover
- Book covers use edge styling and shadow to resemble physical books
- Metric cards and content panels keep contrast soft rather than stark

## Keywords

- Editorial
- Bookstore
- Library-inspired
- Warm neutral
- Premium soft surfaces
- Serif plus sans pairing
