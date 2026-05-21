# Online Voxel Model Renderer

Online Voxel Model Renderer is a .NET 9 web application for uploading, generating, managing, and rendering voxel models directly in the browser.

The project is built as a full-stack application with a Blazor WebAssembly frontend, an ASP.NET Core backend API, and a shared library containing voxel-related data structures such as octrees, Morton indexing, and world/model logic.

## Features

- Browser-based voxel model rendering
- Blazor WebAssembly frontend
- ASP.NET Core backend API
- Shared voxel logic library
- 3D rendering through Ab4d SharpEngine Web
- WebGL support for browser rendering
- Voxel model upload workflow
- Voxel model gallery/viewer components
- Voxel world pages and world API support
- User registration and login
- JWT-based authentication
- Authorized API requests from the frontend
- Backend endpoints for authentication, voxel models, and worlds
- Shared octree data structures
- Morton 3D indexing utilities
- Simplex noise support for procedural generation
- Image processing support through ImageSharp
- MudBlazor-based UI components

## Project Overview

This repository contains three main projects:

- `FE3` - Blazor WebAssembly frontend
- `SEM-Drahos` - ASP.NET Core backend API
- `SharedLib` - shared voxel, octree, world, and utility logic

The frontend is responsible for the user interface, authentication flow, voxel gallery, model viewer, upload page, generation page, and browser-side rendering.

The backend provides API endpoints for authentication, voxel model management, and world management.

The shared library contains reusable logic used by both the frontend and backend, including voxel data structures and octree-related systems.

## Technologies Used

- C#
- .NET 9
- Blazor WebAssembly
- ASP.NET Core
- Ab4d SharpEngine Web
- WebGL
- MudBlazor
- Entity Framework Core
- MongoDB Entity Framework Core Provider
- JWT authentication
- ImageSharp
- SimplexNoise

## Repository Structure

```text
OnlineVoxelModelRenderer/
│
├── FE3/
│   ├── Api/
│   │   ├── Types/
│   │   ├── AuthApi.cs
│   │   ├── ModelsApi.cs
│   │   └── WorldApi.cs
│   │
│   ├── Auth/
│   │   └── Authentication and token-related frontend logic
│   │
│   ├── Components/
│   │   ├── VoxelGalleryItem.razor
│   │   └── VoxelModelViewer.razor
│   │
│   ├── Layout/
│   │   └── Frontend layout components
│   │
│   ├── Native/
│   │   └── Native WebAssembly / SharpEngine integration files
│   │
│   ├── Pages/
│   │   ├── WorldPages/
│   │   ├── Generate.razor
│   │   ├── Home.razor
│   │   ├── Login.razor
│   │   ├── Model.razor
│   │   ├── Register.razor
│   │   └── Upload.razor
│   │
│   ├── Services/
│   │   └── Frontend services for API and application state
│   │
│   ├── VoxelRenderer/
│   │   └── Browser-side voxel rendering logic
│   │
│   ├── wwwroot/
│   │   └── Static frontend assets
│   │
│   ├── App.razor
│   ├── CanvasInterop.cs
│   ├── FE3.csproj
│   ├── Program.cs
│   ├── SharpEngineSceneView.razor
│   └── _Imports.razor
│
├── SEM-Drahos/
│   ├── data/
│   │   ├── entities/
│   │   └── PotDbContext.cs
│   │
│   ├── endpoints/
│   │   ├── AuthEndpoints.cs
│   │   ├── VoxelModelsEndpoints.cs
│   │   └── WorldEndpoints.cs
│   │
│   ├── utils/
│   │   └── Backend utility classes
│   │
│   ├── Program.cs
│   ├── SEM-Drahos.csproj
│   ├── SEM-Drahos.http
│   ├── appsettings.Development.json
│   └── appsettings.json
│
├── SharedLib/
│   ├── Octree/
│   │   ├── FlatOctree.cs
│   │   ├── Morton3D.cs
│   │   └── OctreeNode.cs
│   │
│   ├── UnmanagedStructures/
│   │   └── Shared unmanaged data structures
│   │
│   ├── World/
│   │   └── Shared world and voxel model logic
│   │
│   └── SharedLib.csproj
│
├── SEM-Drahos.sln
├── global.json
└── .gitignore
