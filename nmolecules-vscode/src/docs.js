'use strict';

function getDocumentationCandidates(docsRoot) {
  const normalizedRoot = docsRoot && docsRoot.trim() ? docsRoot.trim().replace(/[\\/]+$/, '') : 'docs';

  return [
    `${normalizedRoot}/architecture.md`,
    `${normalizedRoot}/layer-matrix.md`,
    'README.md'
  ];
}

module.exports = {
  getDocumentationCandidates
};
