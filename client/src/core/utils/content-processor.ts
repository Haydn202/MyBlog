/**
 * Utility functions for processing HTML content
 */

/**
 * Process HTML content to replace non-breaking spaces with regular spaces
 * This allows text to wrap properly at word boundaries
 * @param content - The HTML content string
 * @returns Processed content with &nbsp; replaced with regular spaces
 */
export function processContent(content: string): string {
  if (!content) return '';
  
  // Replace &nbsp; with regular spaces to allow proper word wrapping
  // This is safe because we're only displaying, not editing
  return content.replace(/&nbsp;/g, ' ');
}

