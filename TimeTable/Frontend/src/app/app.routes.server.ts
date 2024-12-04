import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'detail/:id',
    renderMode: RenderMode.Prerender,
    getPrerenderParams: async () => {
      // Fetch all available detail IDs (adjust fetch logic accordingly)
      const id = await fetchDetailIds(); 
      return [{ id: id }];
    }
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];

async function fetchDetailIds(): Promise<string> {
  // Replace this with actual logic to fetch a product ID
  return 'c3d3e586-3ad1-4bfd-a18b-ed0acff8c845';
}
