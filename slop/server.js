'use strict';

const express = require('express');
const fs      = require('fs');
const path    = require('path');

const app  = express();
const PORT = process.env.PORT || 3456;
const DATA = path.join(__dirname, 'data');

app.use(express.json());
app.use(express.static(path.join(__dirname, 'public')));

// ── Type config ───────────────────────────────────────────────────────────────

const TYPES = {
  todos: {
    prefix:   'TODO',
    heading:  '# Developer Todos',
    statuses: ['todo', 'in-progress', 'done', 'blocked'],
  },
  ideas: {
    prefix:   'IDEA',
    heading:  '# Ideas & Features',
    statuses: ['new', 'exploring', 'accepted', 'rejected'],
  },
  discussions: {
    prefix:   'DIS',
    heading:  '# Discussions',
    statuses: ['open', 'in-progress', 'resolved'],
  },
};

// ── Markdown parser ───────────────────────────────────────────────────────────

function parseFile(type) {
  const file = path.join(DATA, `${type}.md`);
  if (!fs.existsSync(file)) return [];
  const content = fs.readFileSync(file, 'utf8');
  const items   = [];

  for (const block of content.split(/\n(?=## )/)) {
    const hm = block.match(/^## ([\w-]+):\s*(.+)/);
    if (!hm) continue;
    const get = key => {
      const m = block.match(new RegExp(`\\*\\*${key}:\\*\\*\\s*([^\n]+)`));
      return m ? m[1].trim() : '';
    };
    items.push({
      id:       hm[1].trim(),
      title:    hm[2].trim(),
      status:   get('Status')   || TYPES[type].statuses[0],
      priority: get('Priority') || '',
      created:  get('Created')  || '',
      tags:     get('Tags')     || '',
      notes:    get('Notes')    || '',
    });
  }
  return items;
}

function serializeItem(item) {
  const lines = [`## ${item.id}: ${item.title}`];
  lines.push(`**Status:** ${item.status}`);
  if (item.priority) lines.push(`**Priority:** ${item.priority}`);
  lines.push(`**Created:** ${item.created}`);
  if (item.tags)  lines.push(`**Tags:** ${item.tags}`);
  if (item.notes) lines.push(`**Notes:** ${item.notes}`);
  return lines.join('\n');
}

function readHeader(type) {
  const file = path.join(DATA, `${type}.md`);
  if (!fs.existsSync(file)) return TYPES[type].heading;
  return fs.readFileSync(file, 'utf8').split(/\n(?=## )/)[0].trimEnd();
}

function saveItems(type, items) {
  const header = readHeader(type);
  const body   = items.length > 0
    ? '\n\n' + items.map(serializeItem).join('\n\n') + '\n'
    : '\n';
  fs.writeFileSync(path.join(DATA, `${type}.md`), header + body, 'utf8');
}

function nextId(type) {
  const items = parseFile(type);
  const nums  = items.map(i => parseInt(i.id.replace(/\D/g, ''), 10)).filter(n => !isNaN(n));
  const max   = nums.length ? Math.max(...nums) : 0;
  return `${TYPES[type].prefix}-${String(max + 1).padStart(3, '0')}`;
}

// ── API ───────────────────────────────────────────────────────────────────────

app.get('/api/config', (_req, res) => res.json(TYPES));

app.get('/api/data', (_req, res) => {
  const result = {};
  for (const type of Object.keys(TYPES)) result[type] = parseFile(type);
  res.json(result);
});

app.post('/api/items/:type', (req, res) => {
  const { type } = req.params;
  if (!TYPES[type]) return res.status(400).json({ error: 'unknown type' });
  const { title, status, priority, tags, notes } = req.body;
  if (!title?.trim()) return res.status(400).json({ error: 'title required' });

  const item = {
    id:       nextId(type),
    title:    title.trim(),
    status:   status || TYPES[type].statuses[0],
    priority: priority || '',
    created:  new Date().toISOString().slice(0, 10),
    tags:     tags  || '',
    notes:    notes || '',
  };
  const items = parseFile(type);
  items.push(item);
  saveItems(type, items);
  res.json({ ok: true, item });
});

app.patch('/api/items/:type/:id', (req, res) => {
  const { type, id } = req.params;
  if (!TYPES[type]) return res.status(400).json({ error: 'unknown type' });
  const items = parseFile(type);
  const idx   = items.findIndex(i => i.id === id);
  if (idx === -1) return res.status(404).json({ error: 'not found' });
  Object.assign(items[idx], req.body);
  saveItems(type, items);
  res.json({ ok: true, item: items[idx] });
});

app.delete('/api/items/:type/:id', (req, res) => {
  const { type, id } = req.params;
  if (!TYPES[type]) return res.status(400).json({ error: 'unknown type' });
  const items    = parseFile(type);
  const filtered = items.filter(i => i.id !== id);
  if (filtered.length === items.length) return res.status(404).json({ error: 'not found' });
  saveItems(type, filtered);
  res.json({ ok: true });
});

// ── Start ─────────────────────────────────────────────────────────────────────

app.listen(PORT, () => {
  console.log('\n  \u{1F331}  Dev Dashboard  \u2192  http://localhost:' + PORT + '\n');
});
