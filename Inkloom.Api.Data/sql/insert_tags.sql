-- Insert generic popular tags into the tags table (PostgreSQL, lowercase-hyphen format)
INSERT INTO "Tags" ("Name")
VALUES
  -- Technology
  ('ai'),
  ('machine-learning'),
  ('data-science'),
  ('python'),
  ('javascript'),
  ('web-development'),
  ('devops'),
  ('cloud'),
  ('sql'),

  -- Business & Productivity
  ('startup'),
  ('entrepreneurship'),
  ('marketing'),
  ('sales'),
  ('project-management'),
  ('productivity'),
  ('business'),
  ('growth'),
  ('strategy'),

  -- Design & Creativity
  ('design'),
  ('ux'),
  ('ui'),
  ('graphic-design'),
  ('branding'),
  ('creativity'),

  -- Lifestyle & Wellness
  ('health'),
  ('fitness'),
  ('nutrition'),
  ('mental-health'),
  ('travel'),
  ('lifestyle'),
  ('personal-development'),

  -- Education & Learning
  ('learning'),
  ('education'),
  ('self-improvement'),
  ('books'),
  ('tutorial'),
  ('how-to'),

  -- Content Types
  ('guide'),
  ('opinion'),
  ('case-study'),
  ('review'),
  ('news'),

  -- Miscellaneous
  ('product'),
  ('tools'),
  ('resources'),
  ('tips'),
  ('insights'),
  ('trends')
ON CONFLICT ("Name") DO NOTHING;