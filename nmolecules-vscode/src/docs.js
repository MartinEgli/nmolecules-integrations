'use strict';

function getDocumentationCandidates(docsRoot) {
  const normalizedRoot = docsRoot && docsRoot.trim() ? docsRoot.trim().replace(/[\\/]+$/, '') : 'docs';

  return [
    `${normalizedRoot}/architecture.md`,
    `${normalizedRoot}/layer-matrix.md`,
    'README.md'
  ];
}

function getRuleCatalogCandidates(docsRoot) {
  const normalizedRoot = docsRoot && docsRoot.trim() ? docsRoot.trim().replace(/[\\/]+$/, '') : 'docs';

  return [
    `${normalizedRoot}/architecture/analyzer-rule-map.md`,
    `${normalizedRoot}/architecture/code-fix-policy.md`,
    `${normalizedRoot}/architecture/diagnostic-writing-guideline.md`,
    `${normalizedRoot}/architecture/dependency-rules.md`
  ];
}

module.exports = {
  getDocumentationCandidates,
  getRuleCatalogCandidates
};
