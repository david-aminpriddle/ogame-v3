import { render } from 'preact'
import { App } from './app.tsx'
import './index.css'

const container = document.getElementById('app') as HTMLElement;
if (container) {
  render(<App />, container);
}
